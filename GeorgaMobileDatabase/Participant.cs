using GeorgaMobileDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{

    public async Task<List<Participant>> GetParticipantsAsync()
    {
        await Init();

        return await _database.Table<Participant>().ToListAsync();
    }

    public async Task<Participant> GetParticipantById(string id)
    {
        await Init();

        return await _database.Table<Participant>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Participant>> GetParticipantByPersonId(string personId)
    {
        await Init();

        return await _database.Table<Participant>().Where(x => x.PersonId == personId).ToListAsync();
    }

    public async Task<List<Participant>> GetParticipantByRoleId(string roleId)
    {
        await Init();

        return await _database.Table<Participant>().Where(x => x.RoleId == roleId).ToListAsync();
    }

    public async Task<String> GetAcceptanceByPersonIdAndRoleId(string personId, string roleId)
    {
        await Init();

        var participation = await _database.Table<Participant>().Where(x => x.PersonId == personId && x.RoleId == roleId).ToListAsync();
        if (participation != null && participation.Count > 0)
            return participation.FirstOrDefault().Acceptance;
        else
            return null;
    }

    public async Task<int> SaveParticipantAsync(Participant participant)
    {
        await Init();

        return await _database.InsertAsync(participant);
    }

    public async Task<int> UpdateParticipantAsync(Participant participant)
    {
        await Init();

        return await _database.UpdateAsync(participant);
    }

    public async Task<int> DeleteParticipantAsync(Participant participant)
    {
        await Init();

        return await _database.DeleteAsync(participant);
    }
}
