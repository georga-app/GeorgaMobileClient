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
