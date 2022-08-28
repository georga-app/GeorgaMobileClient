using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class ProjectViewModel : DatabaseViewModel
{
    public ProjectViewModel()
    {

    }

    private string id;
    public string Id
    {
        get => id;
        set 
        {
            SetProperty(ref id, value);
            Operations = new ObservableCollection<OperationViewModel>();
            var opsTask = Task.Run<List<Operation>>(async () => await Db.GetOperationByProjectId(id));
            var ops = opsTask.Result;
            if (ops == null) return;
            foreach (var op in ops)
                Operations.Add(new OperationViewModel()
                {
                    Id = op.Id,
                    Name = op.Name
                });
        }
    }
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(IconFromOrg))]
    string organizationId;
    [ObservableProperty]
    string name;

    [ObservableProperty]
    ObservableCollection<OperationViewModel> operations;
    /*ObservableCollection<OperationViewModel> Operations
    {
        get  // lazy loading operations
        {
            if (operations != null) return operations;
            operations = new ObservableCollection<OperationViewModel>();
            var opsTask = Task.Run<List<Operation>>(async () => await Db.GetOperationByProjectId(Id));
            var ops = opsTask.Result;
            if (ops == null) return null;
            foreach (var op in ops)
                operations.Add(new OperationViewModel()
                {
                    Id = op.Id,
                    Name = op.Name
                });
            return operations;
        }
    }*/

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
