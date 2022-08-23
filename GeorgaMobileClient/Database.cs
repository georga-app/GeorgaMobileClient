using SQLite;

namespace GeorgaMobileClient
{
    public class Database
    {
        private SQLiteAsyncConnection _database;
        private string _filename;
        private string _password;

        private async Task<bool> Init()
        {
            if (String.IsNullOrEmpty(_password) || String.IsNullOrEmpty(_filename))
                return false;

            if (_database != null)
            {
                return true;
            }

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _filename);
            var options = new SQLiteConnectionString(path, true, _password, postKeyAction: c =>
            {
                c.Execute("PRAGMA cipher_compatibility = 3");
            });
            _database = new SQLiteAsyncConnection(options);
            var tableCreateResult = await _database.CreateTableAsync<Person>();
            if (tableCreateResult != CreateTableResult.Created && tableCreateResult != CreateTableResult.Migrated)
                return false;  // can this even fail?

            return true;
        }

        public Database()
        {
        }

        public async Task<bool> Login(string dbPath, string password)
        {
            _filename = dbPath;
            _password = password;
            return await Init();
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            await Init();

            return await _database.Table<Person>().ToListAsync();
        }

        public async Task<Person> GetPersonByEmail(string email)
        {
            await Init();

            return await _database.Table<Person>().Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<int> SavePersonAsync(Person person)
        {
            await Init();

            return await _database.InsertAsync(person);
        }

        public async Task<int> UpdatePersonAsync(Person person)
        {
            await Init();

            return await _database.UpdateAsync(person);
        }

        public async Task<int> DeletePersonAsync(Person person)
        {
            await Init();

            return await _database.DeleteAsync(person);
        }

        public async Task<List<Person>> QuerySubscribedAsync()
        {
            await Init();

            return await _database.QueryAsync<Person>("SELECT * FROM Person WHERE Subscribed = true");
        }
    }
}
