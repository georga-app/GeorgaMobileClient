using SQLite;

namespace GeorgaMobileClient.Model;

public class Organization
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
}
