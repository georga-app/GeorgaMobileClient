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
