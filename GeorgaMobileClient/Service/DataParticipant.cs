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
using System.Globalization;

namespace GeorgaMobileClient.Service;

public partial class Data
{
    void AddParticipant()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listParticipants {
                    edges {
                  	    node {
                            id
                            person
                            {
                                id
                            }
                            role
                            {
                                id
                            }
                            acceptance
                            adminAcceptance
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveParticipantToDb(dynamic response)
    {
        var listParticipants = response.Data.listParticipants.edges.Children<JObject>();

        var oldParticipants = await _db.GetParticipantsAsync();
        foreach (var oldParticipant in oldParticipants)   // delete old participants in cache
            await _db.DeleteParticipantAsync(oldParticipant);
        foreach (var participant in listParticipants)
        {
            var record = new GeorgaMobileDatabase.Model.Participant()
            {
                Id = participant.node.id,
                PersonId = participant.node.person.id,
                RoleId = participant.node.role.id,
                Acceptance = participant.node.acceptance,
                AdminAcceptance = participant.node.adminAcceptance
            };
            Debug.WriteLine($"Participant record: Id={record.Id} PersonId={record.PersonId} RoleId={record.RoleId} Acceptance={record.Acceptance} AdminAcceptance={record.AdminAcceptance}");
            int recordsWritten = await _db.SaveParticipantAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write participant record: Id={record.Id} PersonId={record.PersonId} RoleId={record.RoleId} Acceptance={record.Acceptance} AdminAcceptance={record.AdminAcceptance}");
        }

        return true;
    }
}
