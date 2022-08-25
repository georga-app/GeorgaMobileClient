using GeorgaMobileClient.View;
using CommunityToolkit.Mvvm.Input;
using GeorgaMobileClient.Interface;

namespace GeorgaMobileClient;

public partial class LoginPage : BasePage, ILoginView
{
    public LoginPage()
	{
        InitializeComponent();
    }

    private void OnRegisterClicked(object sender, EventArgs e)
    {
        repeatPasswordEntry.Focus();
    }

    public void SetFocusToRepeatPassword()
    {
        repeatPasswordEntry.Focus();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if DEBUG
        (BindingContext as LoginViewModel).Login();  // automatically login for debugging purposes
#endif
    }    
}