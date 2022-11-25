using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel;

/// <summary>
/// List all operations associated with a certain project, supplied by query property.
/// </summary>
[QueryProperty(nameof(ProjectId), nameof(ProjectId))]
public partial class OperationsViewModel : DatabaseViewModel
{
    private string projectId;
    public string ProjectId
    {
        get => projectId;
        set 
        {
            SetProperty(ref projectId, value);
            Operations = new ObservableCollection<OperationViewModel>();
            var opsTask = System.Threading.Tasks.Task.Run<List<Operation>>(async () => await Db.GetOperationByProjectId(projectId));
            var ops = opsTask.Result;
            if (ops == null) return;
            foreach (var op in ops)
                Operations.Add(new OperationViewModel()
                {
                    Id = op.Id,
                    Name = op.Name,
                    Description = op.Description
                });
        }
    }
    [ObservableProperty]
    string operationId;
    [ObservableProperty]
    string name;
    [ObservableProperty]
    string description;

    [ObservableProperty]
    ObservableCollection<OperationViewModel> operations;

    public async System.Threading.Tasks.Task OpenTasks(int itemIndex)
    {        
        await Shell.Current.GoToAsync($"/tasks?OperationId={Uri.EscapeDataString(Operations[itemIndex].Id)}");
    }
}
