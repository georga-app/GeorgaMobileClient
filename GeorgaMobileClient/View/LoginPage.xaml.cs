using GeorgaMobileClient.View;
using Microsoft.Toolkit.Mvvm.Input;

namespace GeorgaMobileClient;

public partial class LoginPage : BasePage
{
	public LoginPage()
	{
		InitializeComponent();
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        if (App.Instance is not null)
        {
            App.Instance.User.Authenticated = true;
        }
	}
}