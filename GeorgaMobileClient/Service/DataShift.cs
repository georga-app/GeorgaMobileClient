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
    void AddShift()
    {
        _templates.Add(new DataTemplate()
        {
            Query = """
                listShifts {
                    edges {
                  	    node {
                            id
                            task
                            {
                                id
                            }
                            startTime
                            endTime
                            enrollmentDeadline
                            state
                        }      
                    }
                }
                """
        });
    }

    public async Task<bool> SaveShiftToDb(dynamic response)
    {
        var listShifts = response.Data.listShifts.edges.Children<JObject>();

        var oldShifts = await _db.GetShiftsAsync();
        foreach (var oldShift in oldShifts)   // delete old shifts in cache
            await _db.DeleteShiftAsync(oldShift);
        foreach (var shift in listShifts)
        {
            var record = new GeorgaMobileDatabase.Model.Shift()
            {
                Id = shift.node.id,
                TaskId = shift.node.task.id,
                StartTime = shift.node.startTime,
                EndTime = shift.node.endTime,
                EnrollmentDeadline = shift.node.enrollmentDeadline,
                State = shift.node.state
            };
            Debug.WriteLine($"Shift record: Id={record.Id} TaskId={record.TaskId} StartTime={record.StartTime} EndTime={record.EndTime} EnrollmentDeadline={record.EnrollmentDeadline} State={record.State}");
            int recordsWritten = await _db.SaveShiftAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write shift record: Id={record.Id} TaskId={record.TaskId} StartTime={record.StartTime} EndTime={record.EndTime} EnrollmentDeadline={record.EnrollmentDeadline} State={record.State}");
        }

        return true;
    }
}
