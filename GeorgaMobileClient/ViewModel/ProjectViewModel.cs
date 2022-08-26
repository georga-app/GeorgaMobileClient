using CommunityToolkit.Mvvm.ComponentModel;

namespace GeorgaMobileClient.ViewModel;

public partial class ProjectViewModel : DatabaseViewModel
{
    public ProjectViewModel()
	{
			
	}

    [ObservableProperty]
    string id;
    [ObservableProperty]
    string organizationId;
    [ObservableProperty]
    string name;
}
