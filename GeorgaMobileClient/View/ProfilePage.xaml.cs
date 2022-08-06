using System.Diagnostics;
using GeorgaMobileClient.View;

namespace GeorgaMobileClient;

public partial class ProfilePage : BasePage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("ProfilePage.OnAppearing()");
    }
}