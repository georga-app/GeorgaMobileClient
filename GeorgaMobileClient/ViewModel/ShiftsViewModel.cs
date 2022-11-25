using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(TaskId), nameof(TaskId))]
public partial class ShiftsViewModel : DatabaseViewModel
{
    public ShiftsViewModel()
    {
        ;
    }

    private string taskId;
    public string TaskId
    {
        get => taskId;
        set
        {
            SetProperty(ref taskId, value);
            Shifts = new ObservableCollection<ShiftDetailsViewModel>();
            var opsShift = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Shift>>(async () => await Db.GetShiftByTaskId(taskId));
            var ops = opsShift.Result;
            if (ops == null) return;
            foreach (var op in ops)
                Shifts.Add(new ShiftDetailsViewModel()
                {
                    Id = op.Id,
                    StartTime = op.StartTime.ToString(),
                    EnrollmentDeadline = op.EnrollmentDeadline.ToString(),
                    EndTime = op.EndTime.ToString(),
                    State = op.State
                });
        }
    }
    [ObservableProperty]
    string name;
    [ObservableProperty]
    string description;

    [ObservableProperty]
    ObservableCollection<ShiftDetailsViewModel> shifts;

    public async System.Threading.Tasks.Task OpenRoles(int itemIndex)
    {
        //await Shell.Current.GoToAsync($"/roles?Id={shifts[itemIndex].Id}");
    }
}
