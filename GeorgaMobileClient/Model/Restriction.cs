using SQLite;

namespace GeorgaMobileClient.Model
{
    public class Restriction
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
