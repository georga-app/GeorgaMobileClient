using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;

namespace GeorgaMobileClient.ViewModel;

public partial class ProjectViewModel : DatabaseViewModel
{
    public ProjectViewModel()
    {

    }

    [ObservableProperty]
    string id;
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(IconFromOrg))]
    string organizationId;
    [ObservableProperty]
    string name;

    //[ObservableProperty]
    public ImageSource IconFromOrg
    {
        get
        {
            var iconFromOrg = ImageSource.FromStream(() =>
            {
                var orgTask = Task.Run<Organization>(async () => await Db.GetOrganizationById(OrganizationId));
                var org = orgTask.Result;
                if (org != null)
                {
                    byte[] data = System.Convert.FromBase64String(org.Icon);
                    return new MemoryStream(data);
                }
                else
                    return null;
            });
            return iconFromOrg;
        }
    }

    public string OrganizationName
    {
        get
        {
            var orgTask = Task.Run<Organization>(async () => await Db.GetOrganizationById(OrganizationId));
            var org = orgTask.Result;
            if (org != null)
                return org.Name;
            else
                return null;
        }
    }
}
