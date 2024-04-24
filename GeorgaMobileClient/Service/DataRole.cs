/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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
                            task
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
            dynamic shift = role.node.shift;    // special case shift!=null;
            string? shiftid;                    // only occurs if role
            if (shift != null)                  // information in shift
                shiftid = shift.id;             // differs from the task's one
            else
                shiftid = null;
            dynamic task = role.node.task;      // either shift or task
            string? taskid;                     // can be null
            if (task != null)
                taskid = task.id;
            else
                taskid = null;
            var record = new GeorgaMobileDatabase.Model.Role()
            {
                Id = role.node.id,
                ShiftId = shiftid,
                TaskId = taskid,
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
