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

    public async Task<List<Shift>> GetShiftsAsync()
    {
        await Init();

        return await _database.Table<Shift>().ToListAsync();
    }

    public async Task<Shift> GetShiftById(string id)
    {
        await Init();

        return await _database.Table<Shift>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Shift>> GetShiftByTaskId(string taskId)
    {
        await Init();

        return await _database.Table<Shift>().Where(x => x.TaskId == taskId).ToListAsync();
    }

    public async Task<int> SaveShiftAsync(Shift shift)
    {
        await Init();

        return await _database.InsertAsync(shift);
    }

    public async Task<int> UpdateShiftAsync(Shift shift)
    {
        await Init();

        return await _database.UpdateAsync(shift);
    }

    public async Task<int> DeleteShiftAsync(Shift shift)
    {
        await Init();

        return await _database.DeleteAsync(shift);
    }
}
