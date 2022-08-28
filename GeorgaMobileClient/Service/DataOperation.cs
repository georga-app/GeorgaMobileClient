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

namespace GeorgaMobileClient.Service;

public partial class Data
{
    void AddOperation()
    {
        _templates.Add(new DataTemplate()
        {
            Query = @"
    listOperations {
        edges {
      	    node {
                id
                name
                project
                {
                    id
                }
            }      
   	    }
    }"});
    }

    public async Task<bool> SaveOperationToDb(dynamic response)
    {
        var listOperations = response?.Data?.listOperations.edges.Children<JObject>();

        var oldOps = await _db.GetOperationsAsync();
        foreach (var oldOp in oldOps)   // delete old ops in cache
            await _db.DeleteOperationAsync(oldOp);
        foreach (var operation in listOperations)
        {
            await _db.SaveOperationAsync(new Operation()
            {
                Id = operation.node.id,
                Name = operation.node.name,
                ProjectId = operation.node.project.id
            });
        }

        return true;
    }

}
