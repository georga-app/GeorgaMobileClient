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
		string title;

		[ObservableProperty]
		string firstName;

        [ObservableProperty]
        string lastName;

        [ObservableProperty]
        string mobilePhone;

        [ObservableProperty]
        string qualificationsLanguage;

        [ObservableProperty]
		string result;

		[ICommand]
		async public void Login()
		{
			ValidateAllProperties();
			if (String.IsNullOrEmpty(Email)) 
				IsEmailEmpty = true;
			if (!HasErrors)
            {
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
						id
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

				string token;
				dynamic jwtResponse = null;
                try
				{
					jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
					token = jwtResponse.Data.personAuth.token;
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

				if (App.Instance is not null)
				{
					App.Instance.User.Email = Email;
					App.Instance.User.Password = Password;
					App.Instance.User.Token = token;
					App.Instance.User.Authenticated = true;
				}
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
			if (!IsRepeatPasswordVisible)
			{
				IsRepeatPasswordVisible = true;
				GetLanguageQualifications();
			}
			else
			{
				ValidateAllProperties();
				if (!HasErrors)
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
							$qualificationsLanguage: [ID]
						  ) {
							registerPerson(
							  input: {
								email: $email
								password: $password
								title: $title
								firstName: $firstName
								lastName: $lastName
								mobilePhone: $mobilePhone
								qualificationsLanguage: $qualificationsLanguage
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
							qualificationsLanguage = QualificationsLanguage
						}
					};

					string token;
					dynamic jwtResponse = null;
					try
					{
						jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
						token = jwtResponse.Data.personAuth.token;
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

					if (App.Instance is not null)
					{
						App.Instance.User.Email = Email;
						App.Instance.User.Password = Password;
						App.Instance.User.Token = token;
						App.Instance.User.Authenticated = true;
					}


				}
			}
		}
	}
}
