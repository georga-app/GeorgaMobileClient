using Microsoft.Maui.Controls.Xaml;
using System.Globalization;
using System.Text;

namespace GeorgaMobileClient.View
{
    public class IntToUsericonsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder(new String('\uf007', (int)value), (int)value);     // repeats the fontawesome user icon multiple times
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Length;
        }
    }

    public partial class ShiftsPage : BasePage
	{
		public ShiftsPage()
		{
			InitializeComponent();
		}

        async void OnBackClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
        }

        public async void OnItemTapped(object o, ItemTappedEventArgs e)
        {
            await (BindingContext as ShiftsViewModel).SelectItem(e.ItemIndex);
        }
    }
}
