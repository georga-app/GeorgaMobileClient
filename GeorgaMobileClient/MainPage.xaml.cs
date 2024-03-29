﻿/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GeorgaMobileClient.View;
namespace GeorgaMobileClient;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        //collectionView.ItemsSource = await App.Database.GetPeopleAsync();
        /*
                var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
                graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
                var usernameRequest = new GraphQLRequest
                {
                    Query = @"
                    {
                        listPersons {
                          edges {
                            Person: node {
                              id
                              firstName
                              lastName
                              email
                            }
                          }
                        }
                    }"
                };

                var graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(usernameRequest);
                string data = graphQLResponse.Data.ToString();
                string allPersonsString = graphQLResponse.Data.allPersons.ToString();
                var allPersons = graphQLResponse.Data.allPersons;
        */        /*var edges = allPersons.edges;
                  //List<Person> items = edges.ToObject<List<Person>>();
                  List<Person> items = ((Newtonsoft.Json.Linq.JArray)edges).Select(x => new Person
                  {
                      Id = (string)x["Person"]["id"],
                      FirstName = (string)x["Person"]["firstName"],
                      LastName = (string)x["Person"]["lastName"],
                      Email = (string)x["Person"]["email"],
                  }).ToList();
                  collectionView.ItemsSource = items;*/
    }

    private void OnAuthenticate(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new WebAuthenticatorPage());
	}

	private void OnMaps(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new MapsPage());
	}

	private void OnChangeLanguage(object sender, EventArgs e)
	{
		// get lang as "en"
		string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

		// toggle lang
		if (lang == "de")
		{
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
		}
		else
		{
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
			Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
		}

		// get translated title
		string title = GeorgaMobileClient.Properties.Resources.ChangeLanguage;

		(App.Current as App).MainPage = new AppShell();
	}

	//Person lastSelection;
	void collectionView_SelectionChanged(System.Object sender, SelectionChangedEventArgs e)
	{
		//lastSelection = e.CurrentSelection[0] as Person;

		// nameEntry.Text = lastSelection.Name;
		// subscribed.IsChecked = lastSelection.Subscribed;
	}
}

