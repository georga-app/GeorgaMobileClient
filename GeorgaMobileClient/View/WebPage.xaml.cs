using System.Diagnostics;
using System.Security.Policy;
using VijayAnand.MauiToolkit.Core.ViewModels;

namespace GeorgaMobileClient.View;

public partial class WebPage : BasePage, GeorgaMobileClient.Interface.IWebView
{
    private bool appearingFirstTime = true;

    public WebPage()
    {
        InitializeComponent();

        this.Appearing += async (sender, e) =>
        {
            if (appearingFirstTime)
            {
                appearingFirstTime = false;
                if ((bool)App.Instance.User.Authenticated)
                {
                    string emulatorQueryParam = (DeviceInfo.DeviceType == DeviceType.Virtual) ? "1" : "0";
                    LoadUri($"{(BindingContext as WebViewModel).Endpoint}:3000/auth?id={App.Instance.User.Id}"
                                                                            + $"&token={App.Instance.User.Token}"
                                                                       + $"&adminLevel={App.Instance.User.AdminLevel}"
                                                                          + "&redirect=schedule"
                                                                             + "&embed=1"
                                                                         + $"&emulator={emulatorQueryParam}");
                }
                else
                {
                    // wait screen
                    //await RotateElement(Gearwheel, stopGearwheel);
                }
            }
        };
    }

    // this function is for use by the viewModel
    public void LoadUri(string url)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                webView.Source = new UrlWebViewSource{ Url=url };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        try
        {
            webView.Reload();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}