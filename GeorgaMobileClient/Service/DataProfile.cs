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
using System.Net.Http.Headers;

namespace GeorgaMobileClient.Service;

public partial class Data
{
    void AddProfile()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
            getPersonProfile {
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
            """
        });
    }

    public async Task<bool> SaveProfileToDb(dynamic response)
    {
        var person = response?.Data?.getPersonProfile;

        var oldPersons = await _db.GetPersonsAsync();
        foreach (var oldPerson in oldPersons)   // delete old persons in cache
            await _db.DeletePersonAsync(oldPerson);

        var props = new StringBuilder();
        foreach (var prop in person.properties.edges.Children<JObject>())
        {
            if (props.Length > 0)
                props.Append('|');  // add separator character
            props.Append(prop.node.id.ToString());
        }

        var orgs = new StringBuilder();
        foreach (var org in person.organizationsSubscribed.edges.Children<JObject>())
        {
            if (orgs.Length > 0)
                orgs.Append('|');  // add separator character
            orgs.Append(org.node.id.ToString());
        }

        await _db.SavePersonAsync(new Person()
        {
            Id = person.id,
            Email = person.email,
            FirstName = person.firstName,
            LastName = person.lastName,
            Properties = props.ToString(),
            OrganizationsSubscribed = orgs.ToString()
        });

        return true;
    }

    public async Task<string> UpdateProfileSubscribedOrganizations(string personId, List<string> orgIds)
    {
        var updatePersonRequest = new GraphQLRequest
        {
            Query = """
                mutation UpdatePersonProfile ($organizationsSubscribed: [ID]) {
                    updatePersonProfile(
                        input: {
                            organizationsSubscribed: $organizationsSubscribed
                            }
                        ) {
                            person {
                                id
                            }
                            errors {
                                field
                                messages
                        }
                    }
                }
                """,
    Variables = new
            {
                organizationsSubscribed = orgIds
            }
        };

        dynamic jwtResponse = null;
        try
        {
            jwtResponse = await _graphQLClient.SendQueryAsync<dynamic>(updatePersonRequest);
            if (QueryHasErrors(jwtResponse))
                return Result;
        }
        catch (GraphQLHttpRequestException e)
        {
            Result = e.Content;
            return Result;
        }
        catch (Exception e)
        {
            if (jwtResponse?.Errors?.Length > 0)
                Result = jwtResponse.Errors[0].Message;
            else
                Result = e.Message;
            return Result;
        }


        return "";
    }
}
