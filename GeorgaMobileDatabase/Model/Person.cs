using SQLite;

namespace GeorgaMobileDatabase.Model;

public class Person
{
    [PrimaryKey]
    public string Id { get; set; }          // GraphQL ID
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Indexed]
    public string Email { get; set; }
    public string Properties { get; set; }     // qualifications and restrictions, the person posesses, IDs separated by '|'
    public string OrganizationsSubscribed { get; set; }
}
