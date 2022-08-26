using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Project>> GetProjectsAsync()
    {
        await Init();

        return await _database.Table<Project>().ToListAsync();
    }

    public async Task<Project> GetProjectById(string id)
    {
        await Init();

        return await _database.Table<Project>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveProjectAsync(Project project)
    {
        await Init();

        return await _database.InsertAsync(project);
    }

    public async Task<int> UpdateProjectAsync(Project project)
    {
        await Init();

        return await _database.UpdateAsync(project);
    }

    public async Task<int> DeleteProjectAsync(Project project)
    {
        await Init();

        return await _database.DeleteAsync(project);
    }
}
