using Microsoft.Extensions.Configuration;
using CommunityToolkit.Maui;
using GeorgaMobileClient.View;
using GeorgaMobileClient.ViewModel;
using System.Reflection;

namespace GeorgaMobileClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder
            .UseMauiApp<App>()
            .UseVijayAnandMauiToolkit(ServiceRegistrations.Navigation | ServiceRegistrations.Theme)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("FontAwesomeFreeSolid900.otf", "FASolid");
                fonts.AddFont("FontAwesomeFreeRegular400.otf", "FARegular");
            });
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddSingleton<ProjectsViewModel>();
        builder.Services.AddSingleton<ProjectsPage>();
        builder.Services.AddSingleton<OperationsViewModel>();
        builder.Services.AddSingleton<OperationsPage>();
        builder.Services.AddSingleton<OrganizationsViewModel>();
        builder.Services.AddSingleton<OrganizationsPage>();
        builder.Services.AddSingleton<TasksViewModel>();
        builder.Services.AddSingleton<TasksPage>();

        //var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //var IntExample = MyConfig.GetValue<int>("AppSettings:SampleIntValue");
        //var AppName = MyConfig.GetValue<string>("AppSettings:APP_Name");

        //var config = new ConfigurationBuilder()
        //    .AddJsonFile("GeorgaMobileClient.appsettings.json")
        //    .Build();
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("GeorgaMobileClient.appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        builder.Configuration.AddConfiguration(config);

        var app = builder.Build();

        Services = app.Services;

        return app;
    }

    public static IServiceProvider Services { get; private set; }
}

public class NetworkSettings
{
    public string AndroidEndpoint { get; set; }
    public string OtherPlatformsEndpoint { get; set; }
}
