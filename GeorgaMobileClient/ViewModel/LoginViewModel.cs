using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace GeorgaMobileClient.ViewModel
{
    public partial class LoginViewModel: ObservableValidator
	{
        [ObservableProperty]
        [Required]
        [EmailAddress]
		string email;

		[ObservableProperty]
		bool isEmailValid;

		[ObservableProperty]
		string password;

		[ICommand]
		public void Login()
		{
			ValidateAllProperties();
			;
		}
	}
}
