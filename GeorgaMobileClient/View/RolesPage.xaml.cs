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

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Globalization;
using System.Text;

namespace GeorgaMobileClient.View
{
    public partial class RolesPage : BasePage
    {
        public RolesPage()
        {
            InitializeComponent();
        }

        async void OnBackClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
            await (BindingContext as RolesViewModel).SelectItem(e.ItemIndex);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            // Hack: Get the filter (all shifts vs. my shifts)
            (BindingContext as RolesViewModel).Filter = GetFilterFromRoute();

            base.OnNavigatedTo(args);
        }

        private string GetFilterFromRoute()
        {
            // Hack: As the shell can't define query parameters
            // in XAML, we have to parse the route. 
            // as a convention the last route section defines the category.
            // ugly but works for now :-(
            var route = Shell.Current.CurrentState.Location
                .OriginalString.Split("/").LastOrDefault();
            return route;
        }
    }
}
