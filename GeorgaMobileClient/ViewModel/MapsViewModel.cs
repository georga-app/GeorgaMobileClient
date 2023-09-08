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
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GeorgaMobileClient.ViewModel
{
	public partial class MapsViewModel : DatabaseViewModel
    {
		[ObservableProperty]
		string name;

		string longitude = 10.0429685.ToString();

		public string Longitude
		{
			get => longitude;
			set => SetProperty(ref longitude, value);
		}

		string latitude = 53.5946754.ToString();

		public string Latitude
		{
			get => latitude;
			set => SetProperty(ref latitude, value);
		}

		public string[] NavigationModes { get; } =
		   Enum.GetNames(typeof(NavigationMode));

		int navigationMode;

		public int NavigationMode
		{
			get => navigationMode;
			set => SetProperty(ref navigationMode, value);
		}

		public ICommand MapsCommand { get; }

		public MapsViewModel()
		{
			MapsCommand = new Command(OpenLocation);
		}

		async void OpenLocation()
		{
			await Map.OpenAsync(double.Parse(Latitude), double.Parse(Longitude), new MapLaunchOptions
			{
				Name = Name,
				NavigationMode = (NavigationMode)NavigationMode
			});
		}
	}
}
