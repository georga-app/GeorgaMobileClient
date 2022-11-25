using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(OperationId), nameof(OperationId))]
public partial class TasksViewModel : DatabaseViewModel
{
    public TasksViewModel()
    {
        ;
    }

    private string operationId;
    public string OperationId
    {
        get => operationId;
        set 
        {
            SetProperty(ref operationId, value);
            Tasks = new ObservableCollection<TaskDetailsViewModel>();
            var opsTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Task>>(async () => await Db.GetTaskByOperationId(operationId));
            var ops = opsTask.Result;
            if (ops == null) return;
            foreach (var op in ops)
                Tasks.Add(new TaskDetailsViewModel()
                {
                    Id = op.Id,
                    Name = op.Name,
                    Description = op.Description
                });
        }
    }
    [ObservableProperty]
    string name;
    [ObservableProperty]
    string description;

    [ObservableProperty]
    ObservableCollection<TaskDetailsViewModel> tasks;

    public async System.Threading.Tasks.Task OpenShifts(int itemIndex)
    {
        await Shell.Current.GoToAsync($"/shifts?TaskId={Uri.EscapeDataString(tasks[itemIndex].Id)}"); 
    }
}
