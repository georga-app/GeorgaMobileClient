using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Shift>> GetShiftsAsync()
    {
        await Init();

        return await _database.Table<Shift>().ToListAsync();
    }

    public async Task<Shift> GetShiftById(string id)
    {
        await Init();

        return await _database.Table<Shift>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Shift>> GetShiftByTaskId(string taskId)
    {
        await Init();

        return await _database.Table<Shift>().Where(x => x.TaskId == taskId).ToListAsync();
    }

    public async Task<int> SaveShiftAsync(Shift shift)
    {
        await Init();

        return await _database.InsertAsync(shift);
    }

    public async Task<int> UpdateShiftAsync(Shift shift)
    {
        await Init();

        return await _database.UpdateAsync(shift);
    }

    public async Task<int> DeleteShiftAsync(Shift shift)
    {
        await Init();

        return await _database.DeleteAsync(shift);
    }
}
