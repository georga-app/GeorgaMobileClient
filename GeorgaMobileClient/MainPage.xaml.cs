using GeorgaMobileClient.View;
namespace GeorgaMobileClient;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

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

		var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:80/graphql" : "http://localhost:80/graphql", new NewtonsoftJsonSerializer());
		graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
		var usernameRequest = new GraphQLRequest
		{
			Query = @"
			{
				allPersons {
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
		var edges = allPersons.edges;
		//List<Person> items = edges.ToObject<List<Person>>();
		List<Person> items = ((Newtonsoft.Json.Linq.JArray)edges).Select(x => new Person
		{
			Id = (string)x["Person"]["id"],
			FirstName = (string)x["Person"]["firstName"],
			LastName = (string)x["Person"]["lastName"],
			Email = (string)x["Person"]["email"],
		}).ToList();
		collectionView.ItemsSource = items;
	}

	private void OnAuthenticate(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new WebAuthenticatorPage());
	}

	private void OnMaps(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new MapsPage());
	}

    private void OnGraphQL(object sender, EventArgs e)
    {
		Navigation.PushModalAsync(new GraphQLPage());
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

	Person lastSelection;
	void collectionView_SelectionChanged(System.Object sender, SelectionChangedEventArgs e)
	{
		lastSelection = e.CurrentSelection[0] as Person;

		// nameEntry.Text = lastSelection.Name;
		// subscribed.IsChecked = lastSelection.Subscribed;
	}
}

