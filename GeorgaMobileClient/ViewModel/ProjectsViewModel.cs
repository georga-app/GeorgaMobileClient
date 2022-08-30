using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeorgaMobileClient.ViewModel
{
	public partial class ProjectsViewModel : DatabaseViewModel
    {
		[ObservableProperty]
		ObservableCollection<ProjectDetailsViewModel> projects;

        public ProjectsViewModel()
		{
			Projects = new ObservableCollection<ProjectDetailsViewModel>();
        }

		public async void Init()
		{
			Projects.Clear();
            var projectsFromDb = await Db.GetProjectsAsync();
			foreach (var project in projectsFromDb)
			{
				Projects.Add(new ProjectDetailsViewModel()
				{
					Id = project.Id,
					OrganizationId = project.OrganizationId,
					Name = project.Name,
					Description = project.Description
				});
            }
        }

        public async Task OpenOperations(int itemIndex)
		{
            await Shell.Current.GoToAsync($"/operations?ProjectId={Uri.EscapeDataString(projects[itemIndex].Id)}");
        }
    }
}
