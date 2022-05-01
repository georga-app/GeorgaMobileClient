namespace GeorgaMobileClient;
using CommunityToolkit.Maui;

public partial class App : Application
{
    private static Database database;
    public static Database Database
    {
        get
        {
            if (database == null)
            {
                if (database.Password == null)
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "default.db3"));
            }

            return database;
        }
    }

    private readonly IThemeService? themeService;

    public App()
    {
        InitializeComponent();
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
