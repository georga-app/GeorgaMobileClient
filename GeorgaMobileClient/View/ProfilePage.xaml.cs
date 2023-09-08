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

using System.Diagnostics;
using GeorgaMobileClient.View;
using GeorgaMobileDatabase.Model;

namespace GeorgaMobileClient;

public partial class ProfilePage : BasePage
{
    Dictionary<string, TableSection> qualificationSections;

    public ProfilePage()
    {
        InitializeComponent();
        ViewModel.PersonOptionsChangedEvent += ViewModel_PersonOptionsChangedEvent;
        qualificationSections = new Dictionary<string, TableSection>();
    }

    public void OnAppearing()
    {

    }

    private void ViewModel_PersonOptionsChangedEvent(object sender, ProfileViewModel.PersonOptionsChangedArgs e)
    {
        // remove old qualification sections
        foreach (KeyValuePair<string, TableSection> kv in qualificationSections)
        {
            form.Root.Remove(kv.Value);
            qualificationSections.Remove(kv.Key);
        }

        // add new qualification sections
        foreach (var cat in e.QualificationCategories)
        {
            int numberOfQualificationsInGroup = e.Qualifications.Where(x => x.GroupId == cat.Id).Count();
            if (numberOfQualificationsInGroup > 0)
            {
                string orgName = "";
                if (ViewModel.Organizations.Count > 1)
                {
                    var org = ViewModel.Organizations.Where(x => x.Id == cat.OrganizationId).FirstOrDefault();
                    if (org != null)
                        orgName = " " + GeorgaMobileClient.Properties.Resources.QualificationsFor + " " + org.Name;
                }
                form.Root.Add(qualificationSections[cat.Id] = new TableSection(cat.Name + orgName));
            }
        }

        // populate qualification sections
        foreach (var qual in e.Qualifications)
        {
            SwitchCell boolSwitchCell;
            qualificationSections[qual.GroupId].Add(boolSwitchCell = new SwitchCell()
            {
                Text = qual.Name,
                BindingContext = qual
            });
            boolSwitchCell.SetBinding(SwitchCell.OnProperty, new Binding("Value", BindingMode.TwoWay));
            boolSwitchCell.SetBinding(SwitchCell.IsEnabledProperty, new Binding("IsEditing", BindingMode.OneWay));
        }
    }
}