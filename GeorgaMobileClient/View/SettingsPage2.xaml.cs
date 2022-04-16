namespace GeorgaMobileClient.View
{
    public partial class SettingsPage2 : ContentPage
    {
        public SettingsPage2(SettingsViewModel viewModel)
        {
            //InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
