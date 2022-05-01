using SQLite;

namespace GeorgaMobileClient.Model
{
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int Uuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
