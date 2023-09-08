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

namespace GeorgaMobileClient.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPreferences preferences;
        private readonly IThemeService theme;

        public SettingsViewModel(IPreferences preferences, IThemeService theme)
        {
            this.preferences = preferences;
            this.theme = theme;
            Title = "Settings";
        }

        public int Theme
        {
            get => theme?.Theme ?? 0;
            set
            {
                preferences?.Set(nameof(Theme), value, null);
                theme?.SetTheme();
            }
        }

        public bool UseSystem
        {
            get => Theme == 0;
            set
            {
                if (value)
                {
                    Theme = 0;
                }
            }
        }

        public bool LightTheme
        {
            get => Theme == 1;
            set
            {
                if (value)
                {
                    Theme = 1;
                }
            }
        }

        public bool DarkTheme
        {
            get => Theme == 2;
            set
            {
                if (value)
                {
                    Theme = 2;
                }
            }
        }
    }
}
