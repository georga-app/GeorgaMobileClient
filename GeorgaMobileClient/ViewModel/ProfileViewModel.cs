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
        string email;
        [ObservableProperty]
        string firstName;
        [ObservableProperty]
        string lastName;
        #endregion

        #region view control elements
        public ICommand StoreCommand { protected set; get; }
        public ICommand CancelCommand { protected set; get; }

        [ObservableProperty]
        private bool isStoreEnabled;
        [ObservableProperty]
        private bool isCancelEnabled;
        [ObservableProperty]
        string result;

        public ProfileViewModel(IPreferences preferences, IThemeService theme)
        {
            StoreCommand = new Command(Store);
            CancelCommand = new Command(Cancel);
            IsStoreEnabled = true;
            IsCancelEnabled = true;
            Title = GeorgaMobileClient.Properties.Resources.Profile;
            GetProfileDataFromApi();
            Result = "Nix";
        }

        // store form data
        async void Store()
        {
            if (!IsStoreEnabled) return;
            IsStoreEnabled = false;

            await Shell.Current.GoToAsync("///main");
        }

        async void Cancel()
        {
            if (!IsCancelEnabled) return;
            IsCancelEnabled = false;

            await Shell.Current.GoToAsync("///main");
        }
        #endregion

        #region api stuff
        public async void GetProfileDataFromApi()
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
                graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(usernameRequest);
                if (QueryHasErrors(graphQLResponse))
                {
                    IsStoreEnabled = false;
                    return;
                }
                // string allPersonsString = graphQLResponse.Data.allPersons.ToString();
                var allPersons = graphQLResponse?.Data?.allPersons;
                if (allPersons == null)
                {
                    IsStoreEnabled = false;
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Email = allPersons?.edges[0]?.Person?.email;
                    FirstName = allPersons?.edges[0]?.Person?.firstName;
                    LastName = allPersons?.edges[0]?.Person?.lastName;
                });
            }
            catch (GraphQLHttpRequestException e)
            {
                Result = e.Content;
                return;
            }
            catch (Exception e)
            {
                if (graphQLResponse?.Errors?.Length > 0)
                    Result = graphQLResponse.Errors[0].Message;
                else
                    Result = e.Message;
                return;
            }
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
