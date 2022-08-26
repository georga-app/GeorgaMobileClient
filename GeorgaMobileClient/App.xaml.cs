namespace GeorgaMobileClient;
using CommunityToolkit.Maui;
using GeorgaMobileDatabase;
using GeorgaMobileClient.Service;

public partial class App : Application
{
    private readonly IThemeService? themeService;

    public App()
    {
        InitializeComponent();

        DependencyService.Register<Database>();
        DependencyService.Register<Data>();

        MainPage = new AppShell();

        themeService = AppService.GetService<IThemeService>();

        User = new UserModel();
        // Add logic to check whether the user is authenticated or not and then update this flag
        User.Authenticated = false;
    }

    public static App? Instance => Current as App;

    public UserModel User { get; internal set; }

    protected override void OnStart()
    {
        OnResume();
    }

    protected override void OnResume()
    {
        themeService?.SetTheme();
        RequestedThemeChanged += OnAppThemeChangeRequested;
    }

    protected override void OnSleep()
    {
        RequestedThemeChanged -= OnAppThemeChangeRequested;
    }

    private void OnAppThemeChangeRequested(object? sender, AppThemeChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => themeService?.SetTheme());
    }
}
