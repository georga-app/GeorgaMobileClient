using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class OrganizationsPage : BasePage
	{
		public OrganizationsPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			(BindingContext as OrganizationsViewModel).Init();
		}

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
			//(BindingContext as OrganizationsViewModel).OpenOrganizationDetails(e.ItemIndex);

        }

        void Handle_Toggled(object sender, ToggledEventArgs e)
        {
            (BindingContext as OrganizationsViewModel).ComputeConfirmSubscriptionsVisible();
        }
	}
}
