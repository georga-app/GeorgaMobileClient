using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel
{
    public partial class PersonPropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        public string id;
        [ObservableProperty]
        public string code;
        [ObservableProperty]
        public string groupId;
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public bool value;
        [ObservableProperty]
        public bool isEditing;
    }
}
