using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel
{
    public partial class OperationViewModel : ObservableObject
    {
        [ObservableProperty]
        public string id;
        [ObservableProperty]
        public string name;
    }
}
