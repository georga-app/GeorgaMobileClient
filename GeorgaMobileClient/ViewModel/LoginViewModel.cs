using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using GeorgaMobileDatabase;
using Microsoft.Extensions.Configuration;
using System.Threading.Channels;

namespace GeorgaMobileClient.ViewModel
{
    public partial class LoginViewModel: DatabaseViewModel
    {
        [ObservableProperty]
        [Required]
        [EmailAddress]
		//[AlsoNotifyChangeFor(nameof(IsEmailEmpty))]
		string email = "helper.001@georga.test";

        [ObservableProperty]
		bool isEmailEmpty;

		[ObservableProperty]
		bool isEmailValid;

        [ObservableProperty]
        bool isPasswordValid;

        [ObservableProperty]
        bool isFirstNameValid;

        [ObservableProperty]
        bool isLastNameValid;

		[ObservableProperty]
		bool isMobilePhoneValid;

        [ObservableProperty]
        [MinLength(6)]
        [NotifyPropertyChangedFor(nameof(IsPasswordMatching))]
		string password = "georga";

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(IsPasswordMatching))]
		string repeatPassword;

		[ObservableProperty]
		bool isRepeatPasswordVisible;

		public bool IsPasswordMatching => !IsRepeatPasswordVisible || Password == RepeatPassword;

		[ObservableProperty]
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        string title;

		[ObservableProperty]
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        string firstName;

        [ObservableProperty]
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        string lastName;

        [ObservableProperty]
        [Required]
        [MinLength(10, ErrorMessage = "Phone number should have at least 10 digits.")]
        [MaxLength(20, ErrorMessage = "Phone number is too long.")]
        string mobilePhone;

        [ObservableProperty]
        string qualificationsLanguage;

        [ObservableProperty]
		string result;

        IConfiguration configuration;
        NetworkSettings settings;

        // --- construct ---

        public LoginViewModel()
		{
            configuration = MauiProgram.Services.GetService<IConfiguration>();
            settings = configuration.GetRequiredSection("NetworkSettings").Get<NetworkSettings>();
        }

        // --- events ---

        public override void OnAppearing()
		{
			base.OnAppearing();
			Db.Logout();        // if login page is showing, we consider this a logout
		}

		// --- commands ---

		[RelayCommand]
		async public void Login()
        {
            if (App.Instance is null)
                return;

            if (String.IsNullOrEmpty(Email))  // some atavism that won't harm...
                IsEmailEmpty = true;

			// ValidateAllProperties(); -- no longer : the registration-specific fields are allowed to be invalid for login
			var emailErrors = GetErrors("Email");
            var passwordErrors = GetErrors("Password");
			if (emailErrors?.Count() > 0)
			{
				Result = emailErrors.FirstOrDefault().ErrorMessage;
				return;
			}
            if (passwordErrors?.Count() > 0)
            {
                Result = passwordErrors.FirstOrDefault().ErrorMessage;
                return;
            }

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
			{
				var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());

				var jwtRequest = new GraphQLRequest
				{
					Query = """
					mutation TokenAuth (
						$email: String!
						$password: String!
					)
					{
						personAuth: tokenAuth (
						input: {
							email: $email
							password: $password
						})
						{
							id
							payload
							token
							refreshExpiresIn
						}
					}
					""",
					Variables = new
					{
						email = Email,
						password = Password
					}
				};

				string token = "", id = "";
				dynamic jwtResponse = null;
				try
				{
					jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
					if (QueryHasErrors(jwtResponse))
						return;

					// login, if token has been aquired successfully
					token = jwtResponse.Data.personAuth.token;
                    id = jwtResponse.Data.personAuth.id;

                    _ = await Db.Login(ComputeSha256Hash(Email.ToLower()), Password);
					App.Instance.User.Email = Email;
					App.Instance.User.Password = Password;
                    App.Instance.User.Id = id;
                    App.Instance.User.Token = token;
					App.Instance.User.Authenticated = true;

					D.SetAuthToken();
                    Result = await D.CacheAll();
                    if (Result != "")
                        await Application.Current.MainPage.DisplayAlert("Data Service Error", Result, "OK");
#if DEBUG
                    else
                        // automatically go to page of interest for debugging purposes
                        await Shell.Current.GoToAsync("//projects");
						// await Shell.Current.GoToAsync("//profile");					
#endif
				}
				catch (GraphQLHttpRequestException e)
				{
					Result = e.Content;
					return;
				}
				catch (Exception e)
				{
					if (jwtResponse?.Errors?.Length > 0)
					{
						Result = jwtResponse.Errors[0].Message;
					}
					else
					{
						if (e is SQLite.SQLiteException)
							Result = $"Devices database reports '{e.Message}' -- make sure you entered your password correctly.";
						else
							Result = e.Message;
					}
					return;
				}
			}
			else
			{
                _ = await Db.Login(ComputeSha256Hash(Email.ToLower()), Password);
                App.Instance.User.Email = Email;
                App.Instance.User.Password = Password;
                App.Instance.User.Token = "";			// offline mode
                App.Instance.User.Id = "";
                App.Instance.User.Authenticated = true;
            }
		}

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [RelayCommand]
		public async void Register()
		{
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Result = "Connectivity Error: No internet connection currently.";
                return;
            }

            if (!IsRepeatPasswordVisible)
			{
				IsRepeatPasswordVisible = true;
			}
			else
			{
				ValidateAllProperties();
				if (HasErrors)
				{
					Result = "Please provide valid entries in order to register.";
					return;
				}
				else
				{
					var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());

					var jwtRequest = new GraphQLRequest
					{
						Query = @"
						  mutation RegisterPerson (
							$email: String!
							$password: String!
							$title: String!
							$firstName: String
							$lastName: String
							$mobilePhone: String
						  ) {
							registerPerson(
							  input: {
								email: $email
								password: $password
								title: $title
								firstName: $firstName
								lastName: $lastName
								mobilePhone: $mobilePhone
							  }
							) {
							  id
							  errors {
								field
								messages
							  }
							}
						  }",
						Variables = new
						{
							email = Email,
							password = Password,
							title = Title,
							firstName = FirstName,
							lastName = LastName,
							mobilePhone = MobilePhone,
							
                        }
					};

					dynamic jwtResponse = null;
					try
					{
						jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
						if (QueryHasErrors(jwtResponse.Data.registerPerson))
							return;

                        // var id = jwtResponse.Data.registerPerson.id;

						// Success:
                        IsRepeatPasswordVisible = false;
                        Result = "Success! Please check your inbox for the email we just sent you and click on the activation link in it. Then return here to login. (Sending the email can take a minute; look in your spam folder, if it's not there in time.)";
                        return;
                    }
					catch (GraphQLHttpRequestException e)
					{
						Result = e.Content;
						return;
					}
					catch (Exception e)
					{
						if (jwtResponse?.Errors?.Length > 0)
							Result = jwtResponse.Errors[0].Message;
						else
							Result = e.Message;
						return;
					}
				}
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
    }
}
