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

using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTasksAsync()
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().ToListAsync();
    }

    public async System.Threading.Tasks.Task<GeorgaMobileDatabase.Model.Task> GetTaskById(string id)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTaskByOperationId(string opId)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.OperationId == opId).ToListAsync();
    }

    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTaskByFieldId(string fieldId)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.FieldId == fieldId).ToListAsync();
    }

    public async System.Threading.Tasks.Task<int> SaveTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.InsertAsync(task);
    }

    public async Task<int> UpdateTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.UpdateAsync(task);
    }

    public async Task<int> DeleteTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.DeleteAsync(task);
    }
}
