using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel
{
    public partial class ShiftDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string id;
        [ObservableProperty]
        private string glyph;
        [ObservableProperty]
        private string startTime;
        [ObservableProperty]
        private string endTime;
        [ObservableProperty]
        private string enrollmentDeadline;
        [ObservableProperty]
        private string state;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HelpersVacant))]
        private int helpersNeeded;
        public int HelpersVacant
        {
            get
            {
                int n = helpersNeeded - participantsAccepted;
                if (n < 0) 
                    n = 0;
                return n;
            }
        }
        [ObservableProperty]
        private int participantsAccepted;
        [ObservableProperty]
        private int participantsPending;
        [ObservableProperty]
        private int participantsDeclined;
    }
}
