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
                            enrollmentDeadline
                            endTime
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
                EnrollmentDeadline = shift.node.enrollmentDeadline,
                EndTime = shift.node.endTime,
                State = shift.node.state
            };
            Debug.WriteLine($"Shift record: Id={record.Id}");
            int recordsWritten = await _db.SaveShiftAsync(record);
            if (recordsWritten != 1)
                Debug.WriteLine($"!!! Couldn't write operation record: Id={record.Id}");
        }

        return true;
    }
}
