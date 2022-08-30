using SQLite;
using System;
using System.Resources;

namespace GeorgaMobileDatabase.Model;

public class Task
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [Indexed]
    public string OperationId { get; set; }
    [Indexed]
    public string FieldId { get; set; }
    public string ResourcesRequiredIds { get; set; }
    public string ResourcesDesireableIds { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}