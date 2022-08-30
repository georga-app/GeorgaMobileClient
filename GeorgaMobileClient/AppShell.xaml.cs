using GeorgaMobileClient.View;

namespace GeorgaMobileClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("operations", typeof(OperationsPage));
            Routing.RegisterRoute("tasks", typeof(TasksPage));
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
