/* GeoRGA Mobile Client -- a multi-platform mobile app for the
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
