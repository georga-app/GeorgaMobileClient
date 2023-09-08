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
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{
    private SQLiteAsyncConnection _database;
    private string _filename;
    private string _password;

    private async Task<bool> Init()
    {
        if (String.IsNullOrEmpty(_password) || String.IsNullOrEmpty(_filename))
            return false;

        if (_database != null)
        {
            return true;
        }

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _filename);
        var options = new SQLiteConnectionString(path, true, _password, postKeyAction: c =>
        {
            c.Execute("PRAGMA cipher_compatibility = 3");
        });
        _database = new SQLiteAsyncConnection(options);
        var tableCreateResult = await _database.CreateTableAsync<Operation>();
        tableCreateResult = await _database.CreateTableAsync<Organization>();
        tableCreateResult = await _database.CreateTableAsync<Person>();
        tableCreateResult = await _database.CreateTableAsync<PersonProperty>();
        tableCreateResult = await _database.CreateTableAsync<PersonPropertyGroup>();
        tableCreateResult = await _database.CreateTableAsync<Project>();
        tableCreateResult = await _database.CreateTableAsync<GeorgaMobileDatabase.Model.Task>();
        tableCreateResult = await _database.CreateTableAsync<Shift>();
        tableCreateResult = await _database.CreateTableAsync<Role>();
        tableCreateResult = await _database.CreateTableAsync<Participant>();

        return true;
    }

    public Database()
    {
    }

    public async Task<bool> Login(string dbPath, string password)
    {
        _filename = dbPath;
        _password = password;
        return await Init();
    }

    public void Logout()
    {
        _filename = "";
        _password = "";
        _database = null;
    }
}
