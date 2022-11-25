using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class ShiftsPage : BasePage
	{
		public ShiftsPage()
		{
			InitializeComponent();
		}

        async void OnBackClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
        }

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
            // XXXXXXXXXXXXawait (BindingContext as ProjectsViewModel).OpenRoleDetails(e.ItemIndex);
        }
    }
}