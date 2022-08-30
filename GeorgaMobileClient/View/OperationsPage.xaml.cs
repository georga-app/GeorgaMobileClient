using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class OperationsPage : BasePage
	{
		public OperationsPage()
		{
			InitializeComponent();
		}

        async void OnBackClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
        }

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
            await (BindingContext as OperationsViewModel).OpenTasks(e.ItemIndex);
        }
    }
}
