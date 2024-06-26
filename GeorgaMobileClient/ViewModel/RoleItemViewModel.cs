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

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using GeorgaMobileDatabase.Model;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;

namespace GeorgaMobileClient.ViewModel;

public partial class RoleItemViewModel : DatabaseViewModel
{
    [ObservableProperty]
    private string id;
    [ObservableProperty]
    private string acceptance;
    [ObservableProperty]
    private string roleID;
    [ObservableProperty]
    private string name;
    [ObservableProperty]
    private string description;
    [ObservableProperty]
    private bool isActive;
    [ObservableProperty]
    private bool isTemplate;
    [ObservableProperty]
    private string needsAdminAcceptance;
    [ObservableProperty]
    private string startTime;
    [ObservableProperty]
    private string endTime;
    [ObservableProperty]
    private string enrollmentDeadline;
    [ObservableProperty]
    private string state;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HelpersVacant))]
    private int quantity;
    public int HelpersVacant
    {
        get
        {
            int n = quantity - participantsAccepted;
            if (n < 0) 
                n = 0;
            return n;
        }
    }
    [ObservableProperty]
    private int participantsAccepted;
    [ObservableProperty]
    private int participantsPending;
    [ObservableProperty]
    private int participantsDeclined;
}
