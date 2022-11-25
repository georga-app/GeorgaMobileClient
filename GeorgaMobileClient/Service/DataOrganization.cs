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
            await _db.SaveOrganizationAsync(new Organization()
            {
                Id = organization.node.id,
                Name = organization.node.name,
                Icon = organization.node.icon
            });
        }

        return true;
    }
}
