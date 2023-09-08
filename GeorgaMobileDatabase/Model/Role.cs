/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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
