using SQLite;

namespace GeorgaMobileDatabase.Model;

public class Project
{
    [PrimaryKey]
    public string Id { get; set; }
    public string OrganizationId { get; set; }
    public string Name { get; set; }
}
