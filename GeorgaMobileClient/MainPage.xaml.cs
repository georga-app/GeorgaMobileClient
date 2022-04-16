using GeorgaMobileClient.View;

namespace GeorgaMobileClient;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
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
		//get lang as "en"
		string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

		//toggle lang
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

		//get translated title
		string title = GeorgaMobileClient.Properties.Resources.ChangeLanguage;

		(App.Current as App).MainPage = new AppShell();
	}
}

