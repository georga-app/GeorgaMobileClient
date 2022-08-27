using SQLite;

namespace GeorgaMobileDatabase.Model;

public class Organization
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
}
