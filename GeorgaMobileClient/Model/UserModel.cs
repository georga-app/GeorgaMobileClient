namespace GeorgaMobileClient.Model
{
    public class UserModel : OYYYYbservableObject
    {
        private string _email;
        private string _password;
        private string _token;
        private bool? _authenticated;

        private readonly INavigationService? _navigationService;

        public UserModel()
        {
            _email = string.Empty;
            _password = string.Empty;
            _token = string.Empty;
            _authenticated = null;
            _navigationService = AppService.GetService<INavigationService>();
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        public bool? Authenticated
        {
            get => _authenticated;
            set => SetProperty(ref _authenticated, value, onChanged: async () =>
            {
                if (_navigationService is not null)
                {
                    if (value is true)
                    {
                        await _navigationService.GoToAsync("///main");
                    }
                    else
                    {
                        await _navigationService.GoToAsync("///login");
                    }
                }
            });
        }
    }
}
