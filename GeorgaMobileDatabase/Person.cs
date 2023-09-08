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

    public async Task<List<Person>> GetPersonsAsync()
    {
        await Init();

        return await _database.Table<Person>().ToListAsync();
    }

    public async Task<Person> GetPersonByEmail(string email)
    {
        await Init();

        return await _database.Table<Person>().Where(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<int> SavePersonAsync(Person person)
    {
        await Init();

        return await _database.InsertAsync(person);
    }

    public async Task<int> UpdatePersonAsync(Person person)
    {
        await Init();

        return await _database.UpdateAsync(person);
    }

    public async Task<int> DeletePersonAsync(Person person)
    {
        await Init();

        return await _database.DeleteAsync(person);
    }

    public async Task<List<Person>> QuerySubscribedAsync()
    {
        await Init();

        return await _database.QueryAsync<Person>("SELECT * FROM Person WHERE Subscribed = true");
    }
}
