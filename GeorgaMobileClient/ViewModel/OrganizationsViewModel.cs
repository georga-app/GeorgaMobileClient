using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace GeorgaMobileClient.ViewModel
{
	public partial class OrganizationsViewModel : DatabaseViewModel
    {
		[ObservableProperty]
		ObservableCollection<OrganizationViewModel> organizations;

        [ObservableProperty]
        private bool isConfirmSubscriptionsEnabled;

        [ObservableProperty]
        private bool isConfirmSubscriptionsVisible;

        [ObservableProperty]
        private string result;

        public ICommand ConfirmSubscriptionsCommand { protected set; get; }

        public OrganizationsViewModel()
		{
			Organizations = new ObservableCollection<OrganizationViewModel>();
			ConfirmSubscriptionsCommand = new Command(ConfirmSubscriptions);
            IsConfirmSubscriptionsEnabled = false;
        }

		public async void Init()
		{
            IsConfirmSubscriptionsEnabled = false;

            Organizations.Clear();
            var thisPerson = await Db.GetPersonByEmail(App.Instance.User.Email);
            var organizationsFromDb = await Db.GetOrganizationsAsync();
            var personFromDb = await Db.GetOrganizationsAsync();
            foreach (var organization in organizationsFromDb)
			{
				bool subscriptionState = thisPerson.OrganizationsSubscribed.IndexOf(organization.Id) != -1;
                Organizations.Add(new OrganizationViewModel()
				{
					Id = organization.Id,
					Icon = organization.Icon,
					Name = organization.Name,
					IsSubscribed = subscriptionState,
                    IsSubscribedInitialState = subscriptionState
                });
            }

            IsConfirmSubscriptionsEnabled = true;
        }

        public void ComputeConfirmSubscriptionsVisible()
        {
            bool visible = false;
            foreach (var org in Organizations)
                if (org.IsSubscribed != org.IsSubscribedInitialState)
                {
                    visible = true;
                    break;
                }
            if (IsConfirmSubscriptionsEnabled)
                IsConfirmSubscriptionsVisible = visible;
        }

        public async void OpenOrganizationDetails(int itemIndex)
		{
            //await NavigateAsync(organizations[itemIndex]);
            await Shell.Current.GoToAsync($"/organization?Id={organizations[itemIndex].Id}");
        }

        async void ConfirmSubscriptions()
		{
			if (!IsConfirmSubscriptionsEnabled) return;
            IsConfirmSubscriptionsEnabled = false;
            SetBusy(true);

            var thisPerson = await Db.GetPersonByEmail(App.Instance.User.Email);
            var orgIds = new List<string>();
            foreach (var org in Organizations)
                if (org.IsSubscribed)
                    orgIds.Add(org.Id);
            Result = await D.UpdatePersonSubscribedOrganizations(thisPerson.Id, orgIds);

            if (String.IsNullOrEmpty(Result))
            {
                var orgs = new StringBuilder();
                foreach (var org in orgIds)
                {
                    if (orgs.Length > 0)
                        orgs.Append('|');  // add separator character
                    orgs.Append(org);
                }

                thisPerson.OrganizationsSubscribed = orgs.ToString();
                int recordsUpdated = await Db.UpdatePersonAsync(thisPerson);
                if (recordsUpdated < 1)
                    Result = "Error: Couldn't write subscribed organizations to local database.";
            }

            SetBusy(false);
            IsConfirmSubscriptionsEnabled = true;
        }
    }
}
