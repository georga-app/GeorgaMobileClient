using SQLite;
using System;

namespace GeorgaMobileDatabase.Model;

public class Shift
{
    [PrimaryKey]
    public string Id { get; set; }
    [Indexed]
    public string TaskId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset EnrollmentDeadline { get; set; }
    public string State { get; set; }
}
