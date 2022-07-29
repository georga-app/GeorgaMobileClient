using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using GeorgaMobileClient.Interface;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace GeorgaMobileClient.ViewModel
{
    public partial class LoginViewModel: ObservableValidator
	{
		//private ILoginView view;

		[ObservableProperty]
        [Required]
        [EmailAddress]
		//[AlsoNotifyChangeFor(nameof(IsEmailEmpty))]
		string email = "test1@georga.test";

        [ObservableProperty]
		bool isEmailEmpty;

		[ObservableProperty]
		bool isEmailValid;

		[ObservableProperty]
        bool isFirstNameValid;

        [ObservableProperty]
        bool isLastNameValid;

		[ObservableProperty]
		bool isMobilePhoneValid;

        [ObservableProperty]
        [MinLength(8)]
        [AlsoNotifyChangeFor(nameof(IsPasswordMatching))]
		string password = "sdfwer234";

		[ObservableProperty]
		[AlsoNotifyChangeFor(nameof(IsPasswordMatching))]
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

        // --- constructor ---

        [ICommand]
		async public void Login()
		{
            if (String.IsNullOrEmpty(Email))  // some atavism that wonn't harm...
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

			var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());

			var jwtRequest = new GraphQLRequest
			{
				Query = @"
				mutation TokenAuth (
					$email: String!
					$password: String!
				) {
					personAuth: tokenAuth (
					input: {
						email: $email
						password: $password
					}
					) {
					payload
					token
					refreshExpiresIn
					}
				}",
				Variables = new
				{
					email = Email,
					password = Password
				}
			};

			string token = "";
			dynamic jwtResponse = null;
            try
			{
				jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
                if (QueryHasErrors(jwtResponse.Data.personAuth))
                    return;

				// login, if token has been aquired successfully
                token = jwtResponse.Data.personAuth.token;
                if (App.Instance is not null)
                {
                    App.Instance.User.Email = Email;
                    App.Instance.User.Password = Password;
                    App.Instance.User.Token = token;
                    App.Instance.User.Authenticated = true;
                }
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

		async void GetLanguageQualifications()
		{
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());

            var jwtRequest = new GraphQLRequest
            {
                Query = @"
					query {
						allQualificationsLanguage {
							edges {
								node {
									id
									name
								}
							}
						}
					}"
            };

            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
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

        [ICommand]
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
				GetLanguageQualifications();
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
					var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());

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
				errors = obj.errors;
			}
			catch (Exception e)
			{
                return false;
            }

            if (errors?.Count > 0)
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
