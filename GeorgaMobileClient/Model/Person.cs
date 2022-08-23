using SQLite;

namespace GeorgaMobileClient.Model
{
    public class Person
    {
        [PrimaryKey]
        public string Id { get; set; }          // GraphQL ID
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Indexed]
        public string Email { get; set; }
        public string Options { get; set; }     // qualifications and restrictions, the person posesses, IDs separated by '|'
    }
}
