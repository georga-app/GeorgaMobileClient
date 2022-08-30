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
                Debug.WriteLine($"!!! Couldn't write operation record: Id={record.Id} Name={record.Name} OperationId={record.OperationId} Description={record.Description}");
        }

        return true;
    }

}
