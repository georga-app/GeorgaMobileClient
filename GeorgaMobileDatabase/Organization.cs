using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Organization>> GetOrganizationsAsync()
    {
        await Init();

        return await _database.Table<Organization>().ToListAsync();
    }

    public async Task<Organization> GetOrganizationById(string id)
    {
        await Init();

        return await _database.Table<Organization>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveOrganizationAsync(Organization organization)
    {
        await Init();

        return await _database.InsertAsync(organization);
    }

    public async Task<int> UpdateOrganizationAsync(Organization organization)
    {
        await Init();

        return await _database.UpdateAsync(organization);
    }

    public async Task<int> DeleteOrganizationAsync(Organization organization)
    {
        await Init();

        return await _database.DeleteAsync(organization);
    }
}
