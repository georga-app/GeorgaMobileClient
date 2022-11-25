using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ShiftDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        public string id;
        [ObservableProperty]
        public string startTime;
        [ObservableProperty]
        public string endTime;
        [ObservableProperty]
        public string enrollmentDeadline;
        [ObservableProperty]
        public string state;
    }
}
