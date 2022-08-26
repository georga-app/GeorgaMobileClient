using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class ProjectsPage : BasePage
	{
		public ProjectsPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			(BindingContext as ProjectsViewModel).Init();
		}
    }
}
