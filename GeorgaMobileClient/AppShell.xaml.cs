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

using GeorgaMobileClient.View;

namespace GeorgaMobileClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // register routes that have not already been set in a ShellContent of the XAML
            Routing.RegisterRoute("operations", typeof(OperationsPage));
            Routing.RegisterRoute("tasks", typeof(TasksPage));
            Routing.RegisterRoute("shifts", typeof(ShiftsPage));
            Routing.RegisterRoute("roles", typeof(RolesPage));
            // Routing.RegisterRoute("myshifts", typeof(RolesPage));   // virtual routes to pass filter value because shell
            // Routing.RegisterRoute("allshifts", typeof(RolesPage));  // can't have url parameters in xaml (see RolesPage)
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            if (App.Instance is not null)
            {
                App.Instance.User.Authenticated = false;
            }
        }
    }
}
