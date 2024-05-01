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

using System.Windows.Input;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using GeorgaMobileDatabase.Model;
using Microsoft.Extensions.Configuration;

namespace GeorgaMobileClient.ViewModel
{
    public partial class WebViewModel : DatabaseViewModel
    {
        #region vars
        IConfiguration configuration;
        NetworkSettings settings;
        public string Endpoint;
        #endregion

        #region events

        #endregion

        #region constructor

        public WebViewModel()
        {
            Title = GeorgaMobileClient.Properties.Resources.AdminInterface;
            configuration = MauiProgram.Services.GetService<IConfiguration>();
            settings = configuration.GetRequiredSection("NetworkSettings").Get<NetworkSettings>();
            var isEmulator = DeviceInfo.DeviceType == DeviceType.Virtual;
            Endpoint = DeviceInfo.Platform == DevicePlatform.Android ? (isEmulator ? settings.AndroidEndpointEmulator : settings.AndroidEndpoint) : settings.OtherPlatformsEndpoint;
            if (Endpoint.Length >= 11)
                Endpoint = Endpoint.Substring(0, Endpoint.Length - 11);  // cut ":port/graphql"
        }

        #endregion

        #region commands

        async void Back()
        {
            await Shell.Current.GoToAsync("///main");
        }

        #endregion
    }
}
