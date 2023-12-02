/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(OperationId), nameof(OperationId))]
public partial class TasksViewModel : ModeableViewModel
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
            var thisOperationTask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Operation>(async () => await Db.GetOperationById(operationId));
            var thisOperation = thisOperationTask.Result;
            if (thisOperation != null)
                Title = $"Tasks for {thisOperation.Name}";
            else
                Title = "Tasks";
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
        await Shell.Current.GoToAsync($"/shifts?TaskId={Uri.EscapeDataString(tasks[itemIndex].Id)}&Mode={Mode}"); 
    }
}
