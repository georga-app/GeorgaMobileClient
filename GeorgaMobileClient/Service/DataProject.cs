﻿using GeorgaMobileDatabase.Model;
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
