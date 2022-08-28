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