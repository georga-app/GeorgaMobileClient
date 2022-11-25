using GeorgaMobileDatabase.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Role>> GetRolesAsync()
    {
        await Init();

        return await _database.Table<Role>().ToListAsync();
    }

    public async Task<Role> GetRoleById(string id)
    {
        await Init();

        return await _database.Table<Role>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Role>> GetRoleByShiftId(string shiftId)
    {
        await Init();

        return await _database.Table<Role>().Where(x => x.ShiftId == shiftId).ToListAsync();
    }

    public async Task<int> SaveRoleAsync(Role role)
    {
        await Init();

        return await _database.InsertAsync(role);
    }

    public async Task<int> UpdateRoleAsync(Role role)
    {
        await Init();

        return await _database.UpdateAsync(role);
    }

    public async Task<int> DeleteRoleAsync(Role role)
    {
        await Init();

        return await _database.DeleteAsync(role);
    }
}
