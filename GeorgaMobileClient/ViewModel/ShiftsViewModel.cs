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
            var thisMetatask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Task>(async () => await Db.GetTaskById(taskId));
            var thisTask = thisMetatask.Result;
            if (thisTask != null)
                Title = $"Shifts for {thisTask.Name}";
            else
                Title = "Shifts";
            var opsShift = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Shift>>(async () => await Db.GetShiftByTaskId(taskId));
            var ops = opsShift.Result;
            if (ops == null) return;
            foreach (var op in ops)
            {
                var shift = new ShiftDetailsViewModel()
                {
                    Id = op.Id,
                    StartTime = DateOnly.FromDateTime(op.StartTime.DateTime).ToString() + "    " + TimeOnly.FromDateTime(op.StartTime.DateTime),
                    EnrollmentDeadline = op.EnrollmentDeadline.DateTime.ToString(),
                    EndTime = op.EndTime.DateTime.ToString(),
                    State = op.State
                };
                if (DateOnly.FromDateTime(op.StartTime.DateTime) == DateOnly.FromDateTime(op.EndTime.DateTime))  // only display date, if endtime is not on same day as starttime
                    shift.EndTime = TimeOnly.FromDateTime(op.EndTime.DateTime).ToString();

                // compute helpers needed
                var shiftRolesTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(async () => await Db.GetRoleByShiftId(shift.Id));
                var shiftRoles = shiftRolesTask.Result;
                int helpersNeeded = 0;
                foreach (var role in shiftRoles)
                {
                    helpersNeeded += role.Quantity;
                }
                shift.HelpersNeeded = helpersNeeded;
                Shifts.Add(shift);
            }
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
