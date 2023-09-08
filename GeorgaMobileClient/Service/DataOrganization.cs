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
using GeorgaMobileDatabase;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLite.SQLite3;
using System.Diagnostics;

namespace GeorgaMobileClient.Service;

public partial class Data
{
    void AddOrganization()
    {
        _templates.Add(new DataTemplate()
        {
            Query = @"
    listOrganizations {
        edges {
      	    node {
                id
                name
                icon
            }      
   	    }
    }"});
    }

    public async Task<bool> SaveOrganizationToDb(dynamic response)
    {
        var listOrganizations = response?.Data?.listOrganizations.edges.Children<JObject>();

        var oldOrgs = await _db.GetOrganizationsAsync();
        foreach (var oldOrg in oldOrgs)   // delete old orgs in cache
            await _db.DeleteOrganizationAsync(oldOrg);
        foreach (var organization in listOrganizations)
        {
            var record = new Organization()
            {
                Id = organization.node.id,
                Name = organization.node.name,
                Icon = organization.node.icon
            };
            int recordsWritten = await _db.SaveOrganizationAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write operation record: Id={record.Id} Name={record.Name}");
        }

        return true;
    }
}
