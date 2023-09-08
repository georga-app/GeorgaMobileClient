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
    void AddProject()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listProjects {
                    edges {
                  	    node {
                            id
                            organization{id}
                            name
                            description
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveProjectToDb(dynamic response)
    {
        var listProjects = response?.Data?.listProjects.edges.Children<JObject>();

        var oldProjs = await _db.GetProjectsAsync();
        foreach (var oldProj in oldProjs)   // delete old projects in cache
            await _db.DeleteProjectAsync(oldProj);
        foreach (var project in listProjects)
        {
            var record = new Project()
            {
                Id = project.node.id,
                OrganizationId = project.node.organization.id,
                Name = project.node.name,
                Description = project.node.description
            };
            Debug.WriteLine($"Project record: Id={record.Id} Name={record.Name} OrganizationId={record.OrganizationId} Description={record.Description}");
            int recordsWritten = await _db.SaveProjectAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write project record: Id={record.Id} Name={record.Name} Description={record.Description}");
        }

        return true;
    }
}
