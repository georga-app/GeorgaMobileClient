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
using System.Windows.Input;

namespace GeorgaMobileClient.ViewModel
{
	public class WebAuthenticatorViewModel : DatabaseViewModel
    {
		const string authenticationUrl = "https://xamarin-essentials-auth-sample.azurewebsites.net/mobileauth/";

		public WebAuthenticatorViewModel()
		{
			MicrosoftCommand = new Command(async () => await OnAuthenticate("Microsoft"));
			GoogleCommand = new Command(async () => await OnAuthenticate("Google"));
			FacebookCommand = new Command(async () => await OnAuthenticate("Facebook"));
			AppleCommand = new Command(async () => await OnAuthenticate("Apple"));
		}

		public ICommand MicrosoftCommand { get; }

		public ICommand GoogleCommand { get; }

		public ICommand FacebookCommand { get; }

		public ICommand AppleCommand { get; }

		string accessToken = string.Empty;

		public string AuthToken
		{
			get => accessToken;
			set => SetProperty(ref accessToken, value);
		}

		async Task OnAuthenticate(string scheme)
		{
			try
			{
				WebAuthenticatorResult r = null;

				if (scheme.Equals("Apple", StringComparison.Ordinal)
					&& DeviceInfo.Platform == DevicePlatform.iOS
					&& DeviceInfo.Version.Major >= 13)
				{
					// Make sure to enable Apple Sign In in both the
					// entitlements and the provisioning profile.
					var options = new AppleSignInAuthenticator.Options
					{
						IncludeEmailScope = true,
						IncludeFullNameScope = true,
					};
					r = await AppleSignInAuthenticator.AuthenticateAsync(options);
				}
				else
				{
					var authUrl = new Uri(authenticationUrl + scheme);
					var callbackUrl = new Uri("xamarinessentials://");

					r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
				}

				AuthToken = string.Empty;
				if (r.Properties.TryGetValue("name", out var name) && !string.IsNullOrEmpty(name))
					AuthToken += $"Name: {name}{Environment.NewLine}";
				if (r.Properties.TryGetValue("email", out var email) && !string.IsNullOrEmpty(email))
					AuthToken += $"Email: {email}{Environment.NewLine}";
				AuthToken += r?.AccessToken ?? r?.IdToken;
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("Login canceled.");

				AuthToken = string.Empty;
				await DisplayAlertAsync("Login canceled.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed: {ex.Message}");

				AuthToken = string.Empty;
				await DisplayAlertAsync($"Failed: {ex.Message}");
			}
		}
	}
}