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
    void AddProject()
    {
        _templates.Add(new DataTemplate()
        {
            Query = @"
    allProjects {
        edges {
      	    node {
                id
                organization{id}
                name
            }      
   	    }
    }"});
    }

    public async Task<bool> SaveProjectToDb(dynamic response)
    {
        var allProjects = response?.Data?.allProjects.edges.Children<JObject>();

        var oldProjs = await _db.GetProjectsAsync();
        foreach (var oldProj in oldProjs)   // delete old projects in cache
            await _db.DeleteProjectAsync(oldProj);
        foreach (var project in allProjects)
        {
            await _db.SaveProjectAsync(new Project()
            {
                Id = project.node.id,
                OrganizationId = project.node.organization.id,
                Name = project.node.name
            });
        }

        return true;
    }

}
