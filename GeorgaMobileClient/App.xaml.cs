namespace GeorgaMobileClient;
using CommunityToolkit.Maui;

public partial class App : Application
{
    public Database Db;

    private readonly IThemeService? themeService;

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        themeService = AppService.GetService<IThemeService>();

        Db = new Database();
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
