using SQLite;

namespace GeorgaMobileDatabase.Model;

public class PersonPropertyGroup
{
    [PrimaryKey]
    public string Id { get; set; }
    public string OrganizationId { get; set; }
    public string Codename { get; set; }
    public string Name { get; set; }
}
