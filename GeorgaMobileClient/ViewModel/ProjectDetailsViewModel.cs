using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;

namespace GeorgaMobileClient.ViewModel;

public partial class ProjectDetailsViewModel : DatabaseViewModel
{
    [ObservableProperty]
    public string id;
    [ObservableProperty]
    public string name;
    [ObservableProperty]
    string description;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IconFromOrg))]
    string organizationId;

    public ImageSource IconFromOrg
    {
        get
        {
            return ImageSource.FromStream(() =>
            {
                var orgTask = System.Threading.Tasks.Task.Run<Organization>(async () => await Db.GetOrganizationById(OrganizationId));
                var org = orgTask.Result;
                if (org != null)
                {
                    byte[] data = System.Convert.FromBase64String(org.Icon);
                    return new MemoryStream(data);
                }
                else
                    return null;
            });
        }
    }
}