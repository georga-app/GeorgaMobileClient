﻿using System.Windows.Input;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using GeorgaMobileClient.Interface;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ProfileViewModel : ViewModelBase
    {
        #region form data

        // personal data
        [ObservableProperty]
        string id;
        [ObservableProperty]
        string email;
        [ObservableProperty]
        string firstName;
        [ObservableProperty]
        string lastName;

        // qualifications 
        List<string> qualificationsOfThisPerson;     // temporarily contains all qualifications a person possesses (IDs)

        // general person options
        [ObservableProperty]
        ObservableCollection<QualificationCategory> qualificationCategories;
        [ObservableProperty]
        ObservableCollection<QualificationViewModel> qualifications;
        [ObservableProperty]
        ObservableCollection<Restriction> restrictions;

        #endregion

        #region view control

        public ICommand RefreshCommand { protected set; get; }
        public ICommand EditCommand { protected set; get; }
        public ICommand BackCommand { protected set; get; }
        public ICommand StoreCommand { protected set; get; }
        public ICommand CancelCommand { protected set; get; }

        [ObservableProperty]
        private bool isEditing;
        [ObservableProperty]
        private bool isEditEnabled;
        [ObservableProperty]
        private bool isRefreshEnabled;
        [ObservableProperty]
        private bool isStoreEnabled;
        [ObservableProperty]
        private bool isCancelEnabled;
        [ObservableProperty]
        string result;

        #endregion

        #region events

        public class PersonOptionsChangedArgs
        {
            // public bool QualificationCategoriesHaveChanged;
            public ObservableCollection<QualificationCategory> QualificationCategories;
            // public bool QualificationsHaveChanged;
            public ObservableCollection<QualificationViewModel> Qualifications;
        }

        public delegate void PersonOptionsChangedHandler(object sender, PersonOptionsChangedArgs e);
        public event PersonOptionsChangedHandler PersonOptionsChangedEvent;
        private void RaisePersonOptionsChangedEvent()
        {
            PersonOptionsChangedEvent?.Invoke(null, new PersonOptionsChangedArgs
            {
                QualificationCategories = QualificationCategories,
                Qualifications = Qualifications
            });
        }

        #endregion

        #region constructor

        private Database db;

        public ProfileViewModel()
        {
            this.db = (App.Current as App).Db;
            RefreshCommand = new Command(Refresh);
            EditCommand = new Command(Edit);
            BackCommand = new Command(Back);
            StoreCommand = new Command(Store);
            CancelCommand = new Command(Cancel);
            IsRefreshEnabled = true;
            IsEditEnabled = true;
            IsStoreEnabled = false;
            IsCancelEnabled = true;
            IsEditing = false;
            Title = GeorgaMobileClient.Properties.Resources.Profile;
            Result = "";
            QualificationCategories = new ObservableCollection<QualificationCategory>();
            Qualifications = new ObservableCollection<QualificationViewModel>();
            qualificationsOfThisPerson = new List<string>();

            Refresh();
        }

        #endregion

        #region commands

        async void Refresh()
        {
            if (!IsRefreshEnabled) return;
            IsRefreshEnabled = false;
            Result = "";

            SetBusy(true);

            await Task.WhenAll(GetPersonOptions(), GetProfileDataFromApi());    // do two queries at once

            foreach (string personQualId in qualificationsOfThisPerson)         // set the Value member in 
                foreach (var qual in Qualifications)                            // Qualifications to true
                    if (qual.Id == personQualId)                                // if qualification is 
                    {                                                           // possessed by the person
                        qual.Value = true;
                        break;
                    }

            SetBusy(false);            

            RaisePersonOptionsChangedEvent();
            IsRefreshEnabled = true;
        }

        public async void Edit()
        {
            if (!IsEditEnabled) return;
            IsEditEnabled = false;

            SetBusy(true);
            if (await GetProfileDataFromApi())
            {
                IsEditEnabled = true;
                IsStoreEnabled = true;
                IsCancelEnabled = true;
                IsEditing = true;
                Qualifications.ToList().ForEach(p => p.IsEditing = true);
            }
            SetBusy(false);
        }

        async void Back()
        {
            await Shell.Current.GoToAsync("///main");
        }

        // store form data
        async void Store()
        {
            if (!IsStoreEnabled) return;
            IsStoreEnabled = false;

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Result = "Connectivity Error: No internet connection currently.";
                IsStoreEnabled = true;
                return;
            }

            ValidateAllProperties();
            if (HasErrors)
            {
                Result = "Please provide valid data.";
                IsStoreEnabled = true;
                return;
            }

            SetBusy(true);

            qualificationsOfThisPerson.Clear();
            foreach (var qual in Qualifications)
                if (qual.Value)
                    qualificationsOfThisPerson.Add(qual.Id);

            if (!await SaveToDb())
                Result = "Problem while saving changes to local device.";
            else if (await SendProfileDataToApi())
            {
                IsEditing = false;
                Qualifications.ToList().ForEach(p => p.IsEditing = false);
            }
            else
            {
                IsStoreEnabled = true;
                Qualifications.ToList().ForEach(p => p.IsEditing = true);
            }

            SetBusy(false);
        }

        async void Cancel()
        {
            if (!IsCancelEnabled) return;
                IsCancelEnabled = false;

            Refresh();

            IsRefreshEnabled = true;
            IsEditEnabled = true;
            IsEditing = false;
            Qualifications.ToList().ForEach(p => p.IsEditing = false);
        }

        #endregion

        #region api stuff

        async Task<bool> GetPersonOptions()
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());

            var qualificationsRequest = new GraphQLRequest
            {
                Query = @"
  query GetPersonOptions {
    allQualificationCategories {
      edges {
        node {
          id
          code
          name
        }
      }
    }
    allQualifications {
      edges {
        node {
          id
          name
          qualificationCategory {
            code
          }
        }
      }
    }
  }"
            };


            dynamic qualificationsResponse = null;
            try
            {
                qualificationsResponse = await graphQLClient.SendQueryAsync<dynamic>(qualificationsRequest);
                if (QueryHasErrors(qualificationsResponse))
                    return false;
            }
            catch (GraphQLHttpRequestException e)
            {
                Result = e.Content;
                return false;
            }
            catch (Exception e)
            {
                if (qualificationsResponse?.Errors?.Length > 0)
                    Result = qualificationsResponse.Errors[0].Message;
                else
                    Result = e.Message;
                return false;
            }

            var allQualificationCategories = qualificationsResponse?.Data?.allQualificationCategories.edges.Children<JObject>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                QualificationCategories.Clear();
                foreach (var qualificationCategory in allQualificationCategories)
                {
                    QualificationCategories.Add(new QualificationCategory()
                    {
                        Id = qualificationCategory.node.id,
                        Code = qualificationCategory.node.code,
                        Name = qualificationCategory.node.name
                    });
                }
            });

            var allQualifications = qualificationsResponse?.Data?.allQualifications.edges.Children<JObject>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Qualifications.Clear();
                foreach (var qualification in allQualifications)
                {
                    Qualifications.Add(new QualificationViewModel()
                    {
                        Id = qualification.node.id,
                        Code = qualification.node.qualificationCategory.code,
                        Name = qualification.node.name,
                        Value = false
                    });
                }
            });
            
            /*var allRestrictions = qualificationsResponse?.Data?.allRestrictions.edges.Children<JObject>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Restrictions.Clear();
                foreach (var restriction in allRestrictions)
                {
                    Restrictions.Add(new Restriction()
                    {
                        Id = restriction.node.id,
                        Name = restriction.node.name,
                    });
                }
            });*/

            return true;
        }

        public async Task<bool> GetProfileDataFromApi()
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
            var personRequest = new GraphQLRequest
            {
                Query = @"
query AllPersons ($email: String) {
    allPersons (email: $email) {
	    edges {
	        Person: node {
		        id
		        firstName
		        lastName
		        email
                qualifications {
                    edges {
                        node {
                            id
                        }
                    }
                }
		    }
	    }
    }
}",
                Variables = new
                {
                    email = App.Instance.User.Email
                }
            };

            dynamic graphQLResponse = null;
            try
            {
                graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(personRequest);
                if (QueryHasErrors(graphQLResponse))
                {
                    IsStoreEnabled = false;
                    return false;
                }
                // string allPersonsString = graphQLResponse.Data.allPersons.ToString();
                var allPersons = graphQLResponse?.Data?.allPersons;
                if (allPersons == null)
                {
                    IsStoreEnabled = false;
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Id = allPersons.edges[0].Person?.id;
                    Email = allPersons.edges[0].Person?.email;
                    FirstName = allPersons.edges[0].Person.firstName;
                    LastName = allPersons.edges[0].Person.lastName;
                    var qualificationEdges = allPersons.edges[0].Person.qualifications.edges.Children<JObject>();
                    qualificationsOfThisPerson.Clear();
                    foreach (var qualification in qualificationEdges)
                        qualificationsOfThisPerson.Add(qualification.node.id.ToString());
                });
            }
            catch (GraphQLHttpRequestException e)
            {
                Result = e.Content;
                return false;
            }
            catch (Exception e)
            {
                if (graphQLResponse?.Errors?.Length > 0)
                    Result = graphQLResponse.Errors[0].Message;
                else
                    Result = e.Message;
                return false;
            }
            return true;
        }

        public async Task<bool> SendProfileDataToApi()
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
            var updatePersonRequest = new GraphQLRequest
            {
                Query = @"
  mutation UpdatePerson (
    $id: ID!
    $firstName: String
    $lastName: String
    $qualifications: [ID]
  ) {
    updatePerson(
      input: {
        id: $id
        firstName: $firstName
        lastName: $lastName
        qualifications: $qualifications
      }
    ) {
      person {
        id
      }
      errors {
        field
        messages
      }
    }
  }",
                Variables = new
                {
                    id = Id,
                    firstName = FirstName,
                    lastName = LastName,
                    qualifications = qualificationsOfThisPerson
                }
            };

            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(updatePersonRequest);
                if (QueryHasErrors(jwtResponse))
                    return false;
            }
            catch (GraphQLHttpRequestException e)
            {
                Result = e.Content;
                return false;
            }
            catch (Exception e)
            {
                if (jwtResponse?.Errors?.Length > 0)
                    Result = jwtResponse.Errors[0].Message;
                else
                    Result = e.Message;
                return false;
            }
            return true;
        }

        private bool QueryHasErrors(dynamic obj)
        {
            if (obj == null)
            {
                Result = "Application error (object is null)";  // this shouldn't happen
                return true;
            }
            dynamic errors;
            try
            {
                errors = obj.Errors;
            }
            catch (Exception e)
            {
                return false;
            }

            if (errors?.Length > 0)
            {
                Result = "";
                foreach (dynamic error in errors)
                {
                    try
                    {
                        Result += $"\r\nField '{error.field}': ";
                    }
                    catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
                    {
                        Result += $"\r\n";
                    }

                    try
                    {
                        JArray messages = error.messages;
                        foreach (var message in messages)
                            Result += $"{message}\r\n";
                    }
                    catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
                    {
                        Result += $"{error.Message}\r\n";
                    }    
                }
                return true;
            }
            else
                return false;
        }

        #endregion

        #region database stuff

        public async Task<bool> SaveToDb()
        {
            var thisPerson = await db.GetPersonByEmail(App.Instance.User.Email);

            return true;
        }

        #endregion
    }
}