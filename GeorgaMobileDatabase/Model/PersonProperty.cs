using SQLite;

namespace GeorgaMobileDatabase.Model;

public class PersonProperty
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Code { get; set; }
    public string GroupId { get; set; }
    public string Name { get; set; }
}
