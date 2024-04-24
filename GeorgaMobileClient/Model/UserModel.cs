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

namespace GeorgaMobileClient.Model
{
    public class UserModel : OYYYYbservableObject
    {
        private string _email;
        private string _password;
        private string _id;
        private string _token;
        private string _adminLevel;
        private bool? _authenticated;

        private readonly INavigationService? _navigationService;

        public UserModel()
        {
            _email = string.Empty;
            _password = string.Empty;
            _token = string.Empty;
            _adminLevel = string.Empty;
            _authenticated = null;
            _navigationService = AppService.GetService<INavigationService>();
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        public string AdminLevel
        {
            get => _adminLevel;
            set => SetProperty(ref _adminLevel, value);
        }

        public bool? Authenticated
        {
            get => _authenticated;
            set => SetProperty(ref _authenticated, value, onChanged: async () =>
            {
                if (_navigationService is not null)
                {
                    if (value is true)
                    {
                        await _navigationService.GoToAsync("///main");
                    }
                    else
                    {
                        await _navigationService.GoToAsync("///login");
                    }
                }
            });
        }
    }
}
