using CommunityToolkit.Maui;
using GeorgaMobileClient.View;

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
            });

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddSingleton<ProjectsViewModel>();
        builder.Services.AddSingleton<ProjectsPage>();
        builder.Services.AddSingleton<ProjectViewModel>();
        builder.Services.AddSingleton<ProjectPage>();
        return builder.Build();
	}
}
