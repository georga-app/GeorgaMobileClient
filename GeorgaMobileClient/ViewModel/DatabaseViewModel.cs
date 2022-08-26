using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase;
using GeorgaMobileClient.Service;

namespace GeorgaMobileClient.ViewModel
{
    public class DatabaseViewModel : ViewModelBase
    {
        public Database Db => DependencyService.Get<Database>();
        public Data D => DependencyService.Get<Data>();

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        internal event Func<string, Task> DoDisplayAlert;

        internal event Func<DatabaseViewModel, bool, Task> DoNavigate;

        public Task DisplayAlertAsync(string message)
        {
            return DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;
        }

        public Task NavigateAsync(DatabaseViewModel vm, bool showModal = false)
        {
            return DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;
        }
    }
}
