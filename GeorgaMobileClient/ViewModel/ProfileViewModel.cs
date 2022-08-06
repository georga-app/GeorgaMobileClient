using System.Windows.Input;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ProfileViewModel : ViewModelBase
    {
        #region form data

        [ObservableProperty]
        string id;
        [ObservableProperty]
        string email;
        [ObservableProperty]
        string firstName;
        [ObservableProperty]
        string lastName;

        #endregion

        #region view control elements

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
            Result = "Nix";
            Refresh();
        }

        async void Refresh()
        {
            if (!IsRefreshEnabled) return;
            IsRefreshEnabled = false;

            await GetProfileDataFromApi();

            IsRefreshEnabled = true;
        }

        public async void Edit()
        {
            if (!IsEditEnabled) return;
            IsEditEnabled = false;
            
            if (await GetProfileDataFromApi())
            {
                IsEditEnabled = true;
                IsStoreEnabled = true;
                IsCancelEnabled = true;
                IsEditing = true;
            }
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

            if (await SendProfileDataToApi())
                IsEditing = false;
            else
                IsStoreEnabled = true;
        }

        async void Cancel()
        {
            if (!IsCancelEnabled) return;
                IsCancelEnabled = false;

            IsRefreshEnabled = true;
            IsEditEnabled = true;
            IsEditing = false;
            Refresh();
        }

        #endregion

        #region api stuff

        public async Task<bool> GetProfileDataFromApi()
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
            var usernameRequest = new GraphQLRequest
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
                SetBusy(true);
                graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(usernameRequest);
                SetBusy(false);
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
                    Id = allPersons?.edges[0]?.Person?.id;
                    Email = allPersons?.edges[0]?.Person?.email;
                    FirstName = allPersons?.edges[0]?.Person?.firstName;
                    LastName = allPersons?.edges[0]?.Person?.lastName;
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
  ) {
    updatePerson(
      input: {
        id: $id
        firstName: $firstName
        lastName: $lastName
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
                    lastName = LastName
                }
            };

            dynamic jwtResponse = null;
            try
            {
                SetBusy(true);
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(updatePersonRequest);
                SetBusy(false);
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
                foreach (dynamic error in errors.Children<JObject>())
                {
                    Result += $"\r\nField '{error.field}': ";

                    JArray messages = error.messages;
                    foreach (var message in messages)
                        Result += $"{message}\r\n";
                }
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
