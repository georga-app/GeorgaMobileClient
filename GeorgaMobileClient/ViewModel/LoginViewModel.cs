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
					  $personEmail: String!
					  $personPassword: String!
					) {
					  personAuth: tokenAuth (
						input: {
						  email: $personEmail
						  password: $personPassword
						}
					  ) {
						payload
						refreshExpiresIn
						token
					  }
					}",
					Variables = new
					{
						personEmail = Email,
						personPassword = Password
					}
				};

				string token;
				try
				{
					var jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
					token = jwtResponse.Data.personAuth.token;
				}
				catch (GraphQLHttpRequestException e)
				{
					Result = e.Content;
					return;
				}
				catch (Exception e)
				{
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

		[ICommand]
		public void Register()
		{
			if (!IsRepeatPasswordVisible)
				IsRepeatPasswordVisible = true;
			else
			{
				ValidateAllProperties();
				if (!HasErrors)
				{
					; // do register
				}
			}
		}
	}
}
