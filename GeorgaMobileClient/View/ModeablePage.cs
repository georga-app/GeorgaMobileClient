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

using System;
using System.Threading.Tasks;
using GeorgaMobileClient.ViewModel;
using Microsoft.Maui;

namespace GeorgaMobileClient.View
{
    /// <summary>
    /// Base class for a view that can be operated in 'volunteer'/'enroll' mode or otherwise in 'manage'/'admin' mode
    /// </summary>
    public class ModeablePage : BasePage
    {
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            // Hack: Get the category Id
            GetModeFromRoute();

            base.OnNavigatedTo(args);
        }

        private void GetModeFromRoute()
        {
            // Hack: As the shell can't define query parameters
            // in XAML, we have to parse the route. 
            // as a convention the last route section defines the category.
            // ugly but works for now :-(
            var route = Shell.Current.CurrentState.Location
                .OriginalString.Split("/").LastOrDefault();
            string keyword = "projects";
            if (route.Substring(0, keyword.Length) == keyword)      // only extract mode from "projects" page, otherwise let the QueryProperty attribute do the job
                (BindingContext as ModeableViewModel).Mode = route.Substring(keyword.Length);        // get the part after "projects": either "manage" or "volunteer" and set it as query param
        }
    }
}
