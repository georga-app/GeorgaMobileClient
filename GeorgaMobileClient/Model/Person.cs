using SQLite;

namespace GeorgaMobileClient.Model
{
    public class Person
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
