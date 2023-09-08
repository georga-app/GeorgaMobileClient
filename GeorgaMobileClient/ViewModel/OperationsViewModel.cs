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

            var thisProjectTask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Project>(async () => await Db.GetProjectById(projectId));
            var thisProject = thisProjectTask.Result;
            if (thisProject != null)
                Title = $"Operations for {thisProject.Name}";
            else
                Title = "Operations";
            
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
