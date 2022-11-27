using SQLite;
using System;

namespace GeorgaMobileDatabase.Model;

public class Role
{
    [PrimaryKey]
    public string Id { get; set; }
    [Indexed]
    public string ShiftId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsTemplate { get; set; }
    public bool NeedsAdminAcceptance { get; set; }
    public int Quantity { get; set; }
    public int ParticipantsAccepted { get; set; }
    public int ParticipantsPending { get; set; }
    public int ParticipantsDeclined { get; set; }
}
