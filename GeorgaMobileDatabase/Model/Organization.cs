using SQLite;

namespace GeorgaMobileDatabase.Model;

public class Operation
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
}
