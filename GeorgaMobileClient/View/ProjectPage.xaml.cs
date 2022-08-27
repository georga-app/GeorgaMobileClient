using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class ProjectPage : BasePage
	{
		public ProjectPage()
		{
			InitializeComponent();
		}

        async void OnBackClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
		}
    }
}
