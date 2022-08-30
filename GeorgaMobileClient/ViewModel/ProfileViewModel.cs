using System.Windows.Input;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using GeorgaMobileDatabase.Model;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ProfileViewModel : DatabaseViewModel
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
        ObservableCollection<OrganizationViewModel> organizations;
        [ObservableProperty]
        ObservableCollection<PersonPropertyGroup> qualificationCategories;
        [ObservableProperty]
        ObservableCollection<PersonPropertyViewModel> qualifications;
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
            public ObservableCollection<PersonPropertyGroup> QualificationCategories;
            // public bool QualificationsHaveChanged;
            public ObservableCollection<PersonPropertyViewModel> Qualifications;
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

        public ProfileViewModel()
        {
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
            QualificationCategories = new ObservableCollection<PersonPropertyGroup>();
            organizations = new ObservableCollection<OrganizationViewModel>();
            Qualifications = new ObservableCollection<PersonPropertyViewModel>();
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

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)       // TODO: fallback to offline cache, if other network errors occur
            {
                await System.Threading.Tasks.Task.WhenAll(GetPersonOptions(), GetProfileDataFromApi());    // do two queries at once

                foreach (string personQualId in qualificationsOfThisPerson)         // set the Value member in 
                    foreach (var qual in Qualifications)                            // Qualifications to true
                        if (qual.Id == personQualId)                                // if qualification is 
                        {                                                           // possessed by the person
                            qual.Value = true;
                            break;
                        }
                if (!await SavePersonToDb())
                    Result = "Problem while saving profile data to local device.";
            }
            else
            {
                _ = await GetPersonFromDb();
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

            if (!await SavePersonToDb())
                Result = "Problem while saving changes to local device.";
            else if (await SendProfileDataToApi())
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsEditing = false;
                    Qualifications.ToList().ForEach(p => p.IsEditing = false);
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsStoreEnabled = true;
                    Qualifications.ToList().ForEach(p => p.IsEditing = true);
                });
            }

            SetBusy(false);

            IsEditing = false;

            Refresh();
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
    listPersonPropertyGroups {
      edges {
        node {
          id
          organization { id }
          codename
          name
        }
      }
    }
    listPersonProperties {
      edges {
        node {
          id
          name
          group {
            id
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

            var allQualificationCategories = qualificationsResponse?.Data?.listPersonPropertyGroups.edges.Children<JObject>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                QualificationCategories.Clear();
                foreach (var qualificationCategory in allQualificationCategories)
                {
                    QualificationCategories.Add(new PersonPropertyGroup()
                    {
                        Id = qualificationCategory.node.id,
                        OrganizationId = qualificationCategory.node.organization.id,
                        Codename = qualificationCategory.node.codename,
                        Name = qualificationCategory.node.name
                    });
                }
            });

            var listPersonProperties = qualificationsResponse?.Data?.listPersonProperties.edges.Children<JObject>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Qualifications.Clear();
                foreach (var qualification in listPersonProperties)
                {
                    Qualifications.Add(new PersonPropertyViewModel()
                    {
                        Id = qualification.node.id,
                        GroupId = qualification.node.group.id,
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
                Query = """
                    query getProfile {
                        getProfile {
                            id
                            firstName
                            lastName
                            email
                            properties {
                                edges {
                                    node {
                                        id
                                    }
                                }
                            }
                            organizationsSubscribed {
                                edges {
                                    node {
                                        id
                                        name
                                        icon
                                    }      
                                }
                            }
                        }
                    }
                    """
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
                var getProfile = graphQLResponse?.Data?.getProfile;
                if (getProfile == null)
                {
                    IsStoreEnabled = false;
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Id = getProfile.id;
                    Email = getProfile.email;
                    FirstName = getProfile.firstName;
                    LastName = getProfile.lastName;
                    var qualificationEdges = getProfile.properties.edges.Children<JObject>();
                    qualificationsOfThisPerson.Clear();
                    foreach (var qualification in qualificationEdges)
                        qualificationsOfThisPerson.Add(qualification.node.id.ToString());
                    var organizationEdges = getProfile.organizationsSubscribed.edges.Children<JObject>();
                    organizations.Clear();
                    foreach (var organization in organizationEdges)
                    {
                        organizations.Add(new OrganizationViewModel()
                        {
                            Id = organization.node.id.ToString(),
                            Name = organization.node.name.ToString(),
                            Icon = organization.node.icon.ToString()
                        });
                    }
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
  mutation UpdateProfile (
    $firstName: String
    $lastName: String
    $properties: [ID]
  ) {
    updateProfile(
      input: {
        firstName: $firstName
        lastName: $lastName
        properties: $properties
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
                    firstName = FirstName,
                    lastName = LastName,
                    properties = qualificationsOfThisPerson
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

        public async Task<bool> SavePersonToDb()
        {
            if (String.IsNullOrEmpty(Id))  // is there valid data?
                return false;

            bool createNewRecord = false;
            var thisPerson = await Db.GetPersonByEmail(App.Instance.User.Email);    // record exists? if not,
            if (thisPerson == null)                                                 // create new db entry
            {
                thisPerson = new Person();
                thisPerson.Email = App.Instance.User.Email;
                createNewRecord = true;
            }

            thisPerson.Id = Id;
            thisPerson.FirstName = FirstName;
            thisPerson.LastName = LastName;
            thisPerson.Properties = "";

            int recordsCreated = 0;
            int recordsUpdated = 0;
            if (createNewRecord)
                recordsCreated = await Db.SavePersonAsync(thisPerson);
            else
                recordsUpdated = await Db.UpdatePersonAsync(thisPerson);
            Console.WriteLine($"{recordsCreated} record newly created, {recordsUpdated} existing records updated.");

            return true;
        }

        /*public async Task<bool> SaveOrganizationsToDb()
        {
            if (String.IsNullOrEmpty(Id))  // is there valid data?
                return false;

            var oldOrgs = await Db.GetOrganizationsAsync();
            foreach (var oldOrg in oldOrgs)   // delete old orgs in cache
                await Db.DeleteOrganizationAsync(oldOrg);
            foreach (var newOrg in Organizations)
                await Db.SaveOrganizationAsync(new Organization { Id = newOrg.Id, Name = newOrg.Name, Icon = newOrg.Icon });
            return true;
        }*/

        public async Task<bool> GetPersonFromDb()
        {
            var thisPerson = await Db.GetPersonByEmail(App.Instance.User.Email);    // record exists?
            if (thisPerson != null)
            {
                Id = thisPerson.Id;
                FirstName = thisPerson.FirstName;
                LastName = thisPerson.LastName;
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
