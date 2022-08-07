using System.Diagnostics;
using GeorgaMobileClient.View;

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
            form.Root.Add(qualificationSections[cat.Code] = new TableSection(cat.Name));
        }

        // populate qualification sections
        foreach (var qual in e.Qualifications)
        {
            SwitchCell boolSwitchCell;
            qualificationSections[qual.Code].Add(boolSwitchCell = new SwitchCell()
            {
                Text = qual.Name
            });
            //boolSwitchCell.SetBinding(SwitchCell.OnProperty,
            //    new Binding($"Values[{id}]", BindingMode.TwoWay, string2boolConverter));
        }
    }
}