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

using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core;
using OneSignalSDK.DotNet.Core.Debug;
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

        // Push notification with OneSignal service
        if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            // Enable verbose OneSignal logging to debug issues if needed.
            // OneSignal.Debug.LogLevel = LogLevel.VERBOSE;

            // OneSignal Initialization
            OneSignal.Initialize("b36a32c7-be08-421f-a369-10abe7de8082");

            // RequestPermissionAsync will show the notification permission prompt.
            // We recommend removing the following code and instead using an In-App Message to prompt for notification permission
            // (See step 5 on https://documentation.onesignal.com/docs/net-sdk-setup)
            OneSignal.Notifications.RequestPermissionAsync(true);
        }

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
