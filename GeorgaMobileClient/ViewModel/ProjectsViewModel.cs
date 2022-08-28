using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeorgaMobileClient.ViewModel
{
	public partial class ProjectsViewModel : DatabaseViewModel
    {
		[ObservableProperty]
		ObservableCollection<ProjectViewModel> projects;


        [ObservableProperty]
        private bool isManageSubscriptionsEnabled;
        public ICommand ManageSubscriptionsCommand { protected set; get; }

        public ProjectsViewModel()
		{
			Projects = new ObservableCollection<ProjectViewModel>();
			ManageSubscriptionsCommand = new Command(ManageSubscriptions);
			IsManageSubscriptionsEnabled = true;
        }

		public async void Init()
		{
			Projects.Clear();
            var projectsFromDb = await Db.GetProjectsAsync();
			foreach (var project in projectsFromDb)
			{
				Projects.Add(new ProjectViewModel()
				{
					Id = project.Id,
					OrganizationId = project.OrganizationId,
					Name = project.Name
				});
            }
        }

        public async void OpenProjectDetails(int itemIndex)
		{
            //await NavigateAsync(projects[itemIndex]);
            await Shell.Current.GoToAsync($"/project?Id={projects[itemIndex].Id}");
        }

        async void ManageSubscriptions()
		{
			if (!IsManageSubscriptionsEnabled) return;
            IsManageSubscriptionsEnabled = false;

			// await NavigateAsync ...
			// await NavigateAsync(projects[0]);

            IsManageSubscriptionsEnabled = true;
        }
    }
}
