using Microsoft.Maui.Controls.Xaml;

namespace GeorgaMobileClient.View
{
	public partial class TasksPage : BasePage
	{
		public TasksPage()
		{
			InitializeComponent();
		}

        async void OnBackClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
        }

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
            // XXXXXXXXXXXXawait (BindingContext as ProjectsViewModel).OpenTaskDetails(e.ItemIndex);
        }
    }
}
