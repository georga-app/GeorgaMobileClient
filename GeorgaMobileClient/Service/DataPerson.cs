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
    void AddPerson()
    {
        _templates.Add(new DataTemplate()
        {
            Query = @"
    allPersons (email: $email) {
	    edges {
	        Person: node {
		        id
		        firstName
		        lastName
		        email
                properties {
                    edges {
                        node {
                            id
                        }
                    }
                }
                organizationsSubscribed {
                    edges {
                        node {
                            id
                            name
                            icon
                        }
                    }
                }
		    }
	    }
    }"});
    }

    public async Task<bool> SavePersonToDb(dynamic response)
    {
        var allPersons = response?.Data?.allPersons.edges.Children<JObject>();

        var oldProjs = await _db.GetPersonsAsync();
        foreach (var oldProj in oldProjs)   // delete old persons in cache
            await _db.DeletePersonAsync(oldProj);
        foreach (var person in allPersons)
        {
            var props = new StringBuilder();
            foreach (var prop in person.properties.edges.Children<JObject>())
            {
                if (props.Length > 0)
                    props.Append('|');  // add separator character
                props.Append(prop.node.id.ToString());
            }
            var organizationEdges = allPersons.edges[0].Person.organizationsSubscribed.edges.Children<JObject>();

            var orgs = new StringBuilder();
            foreach (var org in person.organizationsSubscribed.edges.Children<JObject>())
            {
                if (orgs.Length > 0)
                    orgs.Append('|');  // add separator character
                orgs.Append(org.node.id.ToString());
            }

            var p = new Person()
            {
                Id = person.node.id,
                Email = person.node.email,
                FirstName = person.node.firstName,
                LastName = person.lastName,
                Properties = props.ToString(),
                OrganizationsSubscribed = orgs.ToString()
            };
            await _db.SavePersonAsync(p);
        }

        return true;
    }
}
