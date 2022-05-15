using GeorgaMobileClient.View;
using Microsoft.Toolkit.Mvvm.Input;
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
}