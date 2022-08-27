using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Person>> GetPersonsAsync()
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
