using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTasksAsync()
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().ToListAsync();
    }

    public async System.Threading.Tasks.Task<GeorgaMobileDatabase.Model.Task> GetTaskById(string id)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTaskByOperationId(string opId)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.OperationId == opId).ToListAsync();
    }

    public async System.Threading.Tasks.Task<List<GeorgaMobileDatabase.Model.Task>> GetTaskByFieldId(string fieldId)
    {
        await Init();

        return await _database.Table<GeorgaMobileDatabase.Model.Task>().Where(x => x.FieldId == fieldId).ToListAsync();
    }

    public async System.Threading.Tasks.Task<int> SaveTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.InsertAsync(task);
    }

    public async Task<int> UpdateTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.UpdateAsync(task);
    }

    public async Task<int> DeleteTaskAsync(GeorgaMobileDatabase.Model.Task task)
    {
        await Init();

        return await _database.DeleteAsync(task);
    }
}
