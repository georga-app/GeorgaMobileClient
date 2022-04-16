using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace GeorgaMobileClient.ViewModel
{
	public class MapsViewModel : BaseViewModel
	{
		string name = "Thomas' home";

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

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
