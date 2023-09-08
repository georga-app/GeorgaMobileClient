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
    void AddTask()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listTasks {
                    edges {
                  	    node {
                            id
                            name
                            description
                            operation
                            {
                                id
                            }
                            field
                            {
                                id
                            }
                            resourcesRequired {
                                edges {
                  	                node {
                                        id
                                    }
                                }
                            }
                            resourcesDesirable {
                                edges {
                	                  node {
                                        id
                                    }
                                }
                            }
                            startTime
                            endTime
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveTaskToDb(dynamic response)
    {
        var listTasks = response.Data.listTasks.edges.Children<JObject>();

        var oldTasks = await _db.GetTasksAsync();
        foreach (var oldTask in oldTasks)   // delete old tasks in cache
            await _db.DeleteTaskAsync(oldTask);
        foreach (var task in listTasks)
        {
            var resourcesRequiredIds = new StringBuilder();
            foreach (var resReq in task.node.resourcesRequired.edges.Children<JObject>())
            {
                if (resourcesRequiredIds.Length > 0)
                    resourcesRequiredIds.Append('|');  // add separator character
                resourcesRequiredIds.Append(resReq.id.ToString());
            }

            var resourcesDesirableIds = new StringBuilder();
            foreach (var resReq in task.node.resourcesDesirable.edges.Children<JObject>())
            {
                if (resourcesDesirableIds.Length > 0)
                    resourcesDesirableIds.Append('|');  // add separator character
                resourcesDesirableIds.Append(resReq.id.ToString());
            }

            var record = new GeorgaMobileDatabase.Model.Task()
            {
                Id = task.node.id,
                Name = task.node.name,
                Description = task.node.description,
                OperationId = task.node.operation.id,
                FieldId = task.node.field.id,
                ResourcesRequiredIds = resourcesRequiredIds.ToString(),
                ResourcesDesireableIds = resourcesDesirableIds.ToString(),
                //StartTime = task.node.startTime,
                //EndTime = task.node.endTime
            };
            Debug.WriteLine($"Task record: Id={record.Id} Name={record.Name} OperationId={record.OperationId} Description={record.Description}");
            int recordsWritten = await _db.SaveTaskAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write task record: Id={record.Id} Name={record.Name} OperationId={record.OperationId} Description={record.Description}");
        }

        return true;
    }
}
