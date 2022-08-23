using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ViewModelBase : ObservableValidator
    {
        [ObservableProperty]
        private bool isBusy;        
        [ObservableProperty]
        private string title = string.Empty;

        protected void SetBusy(bool value)
        {
            IsBusy = value;
        }
    }
}