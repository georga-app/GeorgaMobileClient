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
    void AddRole()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listRoles {
                    edges {
                  	    node {
                            id
                            shift
                            {
                                id
                            }
                            name
                            description
                            isActive
                            isTemplate
                            needsAdminAcceptance
                            quantity
                            participantsAccepted
                            participantsPending 
                            participantsDeclined
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveRoleToDb(dynamic response)
    {
        var listRoles = response.Data.listRoles.edges.Children<JObject>();

        var oldRoles = await _db.GetRolesAsync();
        foreach (var oldRole in oldRoles)   // delete old roles in cache
            await _db.DeleteRoleAsync(oldRole);
        foreach (var role in listRoles)
        {
            var record = new GeorgaMobileDatabase.Model.Role()
            {
                Id = role.node.id,
                ShiftId = role.node.shift.id,
                Name = role.node.name,
                Description = role.node.description,
                IsActive = role.node.isActive,
                IsTemplate = role.node.isTemplate,
                NeedsAdminAcceptance = role.node.needsAdminAcceptance,
                Quantity = role.node.quantity,
                ParticipantsAccepted = role.node.participantsAccepted,
                ParticipantsPending = role.node.participantsPending,
                ParticipantsDeclined = role.node.participantsDeclined
            };
            Debug.WriteLine($"Role record: Id={record.Id}");
            int recordsWritten = await _db.SaveRoleAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write role record: Id={record.Id} Name={record.Name} Description={record.Description}");
        }

        return true;
    }
}
