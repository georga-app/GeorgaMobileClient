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
    void AddOperation()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listOperations {
                    edges {
                  	    node {
                            id
                            name
                            description
                            project
                            {
                                id
                            }
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveOperationToDb(dynamic response)
    {
        var listOperations = response?.Data?.listOperations.edges.Children<JObject>();

        var oldOps = await _db.GetOperationsAsync();
        foreach (var oldOp in oldOps)   // delete old ops in cache
            await _db.DeleteOperationAsync(oldOp);
        foreach (var operation in listOperations)
        {
            var record = new Operation()
            {
                Id = operation.node.id,
                Name = operation.node.name,
                ProjectId = operation.node.project.id,
                Description = operation.node.description
            };
            Debug.WriteLine($"Operation record: Id={record.Id} Name={record.Name} ProjectId={record.ProjectId} Description={record.Description}");
            int recordsWritten = await _db.SaveOperationAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write operation record: Id={record.Id} Name={record.Name} ProjectId={record.ProjectId} Description={record.Description}");
        }

        return true;
    }
}
