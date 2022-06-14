using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Windows.Input;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace GeorgaMobileClient.ViewModel
{
	public class allPersons
	{
		public string edges { get; set; }
	}
	public class node
	{
		public string email { get; set; }
	}

	public class GraphQLViewModel : BaseViewModel
	{
		string username = "Thomas' home";

		public string Username
		{
			get => username;
			set => SetProperty(ref username, value);
		}

		public ICommand GraphQLCommand { get; }

		public GraphQLViewModel()
		{
			GraphQLCommand = new Command(SendQuery);
		}

		async void SendQuery()
		{
			var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());

			var jwtRequest = new GraphQLRequest
			{
				Query = "mutation {" +
							"tokenAuth(email:\"admin@georga.app\", password:\"verysafePassword\")" +
							"{" +
								"token" +
							"}" +
						"}"
			};
			var jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
			string token = jwtResponse.Data.tokenAuth.token;
			graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", token);

			var usernameRequest = new GraphQLRequest
			{
				Query = @"
				{
					allPersons {
						edges {
						  node {
							email
						  }
						}
					}
				}"
			};
/*			var graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(usernameRequest);
			string data = graphQLResponse.Data.ToString();
			string allPersonsString = graphQLResponse.Data.allPersons.ToString();
			var allPersons = graphQLResponse.Data.allPersons;
			var edges = allPersons.edges[0];
			var node = edges.node;
			var email = node.email;
*/			//List<Node> n = graphQLResponse.Data.allPersons.edges;
			//List<Node> n = graphQLResponse.Data.GetDataFieldAs<List<Node>>("edges");
		}
	}
}
