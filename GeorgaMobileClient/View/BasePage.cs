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
    public class BasePage : ContentPage
    {
        public BasePage()
        {
            // NavigationPage.SetBackButtonTitle(this, "Back");
            if (Device.Idiom == TargetIdiom.Watch)
                NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetupBinding(BindingContext);
        }

        protected override void OnDisappearing()
        {
            TearDownBinding(BindingContext);

            base.OnDisappearing();
        }

        protected void SetupBinding(object bindingContext)
        {
            if (bindingContext is DatabaseViewModel vm)
            {
                vm.DoDisplayAlert += OnDisplayAlert;
                vm.DoNavigate += OnNavigate;
                vm.OnAppearing();
            }
        }

        protected void TearDownBinding(object bindingContext)
        {
            if (bindingContext is DatabaseViewModel vm)
            {
                vm.OnDisappearing();
                vm.DoDisplayAlert -= OnDisplayAlert;
                vm.DoNavigate -= OnNavigate;
            }
        }

        Task OnDisplayAlert(string message)
        {
            return DisplayAlert(Title, message, "OK");
        }

        Task OnNavigate(DatabaseViewModel vm, bool showModal)
        {
            var name = vm.GetType().Name;
            name = name.Replace("ViewModel", "Page");

            var ns = GetType().Namespace;
            var pageType = Type.GetType($"{ns}.{name}");

            var page = (BasePage)Activator.CreateInstance(pageType);
            page.BindingContext = vm;

            return showModal
                ? Navigation.PushModalAsync(page)
                : Navigation.PushAsync(page);
        }
    }
}
