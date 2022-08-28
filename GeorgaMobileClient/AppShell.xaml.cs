using GeorgaMobileClient.View;

namespace GeorgaMobileClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("project", typeof(ProjectPage));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            if (App.Instance is not null)
            {
                App.Instance.User.Authenticated = false;
            }
        }
    }
}
