using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;

namespace GeorgaMobileClient.ViewModel
{
    public partial class OrganizationViewModel : ObservableObject
    {
        [ObservableProperty]
        string id;
        [ObservableProperty]
        string name;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IconImageSource))]
        string icon;
        [ObservableProperty]
        bool isSubscribed;
        [ObservableProperty]
        bool isSubscribedInitialState;

        public ImageSource IconImageSource
        {
            get
            {
                return ImageSource.FromStream(() =>
                {
                    if (String.IsNullOrEmpty(Icon))
                        return null;

                    byte[] data = System.Convert.FromBase64String(Icon);
                    return new MemoryStream(data);
                });
            }
        }
    }
}
