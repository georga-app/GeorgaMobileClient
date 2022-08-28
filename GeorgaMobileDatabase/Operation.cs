using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Operation>> GetOperationsAsync()
    {
        await Init();

        return await _database.Table<Operation>().ToListAsync();
    }

    public async Task<Operation> GetOperationById(string id)
    {
        await Init();

        return await _database.Table<Operation>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<List<Operation>> GetOperationByProjectId(string projectId)
    {
        await Init();

        return await _database.Table<Operation>().Where(x => x.ProjectId == projectId).ToListAsync();
    }

    public async Task<int> SaveOperationAsync(Operation organization)
    {
        await Init();

        return await _database.InsertAsync(organization);
    }

    public async Task<int> UpdateOperationAsync(Operation organization)
    {
        await Init();

        return await _database.UpdateAsync(organization);
    }

    public async Task<int> DeleteOperationAsync(Operation organization)
    {
        await Init();

        return await _database.DeleteAsync(organization);
    }
}
