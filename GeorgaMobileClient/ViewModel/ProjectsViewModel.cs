using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace GeorgaMobileClient.ViewModel
{
	public partial class ProjectsViewModel : DatabaseViewModel
    {
		[ObservableProperty]
		ObservableCollection<ProjectViewModel> projects;

        public ProjectsViewModel()
		{
			Projects = new ObservableCollection<ProjectViewModel>();
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
	}
}
