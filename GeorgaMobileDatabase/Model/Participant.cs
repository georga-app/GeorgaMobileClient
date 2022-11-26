using SQLite;
using System;

namespace GeorgaMobileDatabase.Model;

public class Participant
{
    [PrimaryKey]
    public string Id { get; set; }
    [Indexed]
    public string PersonId { get; set; }
    [Indexed]
    public string RoleId { get; set; }
    public string Acceptance { get; set; }
    public string AdminAcceptance { get; set; }
}
