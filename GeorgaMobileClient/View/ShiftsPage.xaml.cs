using Microsoft.Maui.Controls;
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

    public class AcceptanceToGlyphConverter : IValueConverter
    {
        const string itemGlyphCheck = "\uf274";
        const string itemGlyphPlus = "\uf271";
        const string itemGlyphMinus = "\uf272";
        const string itemGlyphX = "\uf273";
        const string itemGlyphNeutral = "\uf133";
        //const string itemGlyph = "\uf133";  // or f073 ?

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return itemGlyphNeutral;

            switch (value.ToString())
            {
                case "ACCEPTED": return itemGlyphCheck;
                case "ACCEPTING": return itemGlyphPlus;     // transient --> ACCEPTED
                case "PENDING": return itemGlyphPlus;
                case "DECLINED": return itemGlyphX;
                case "DECLINING": return itemGlyphMinus;    // transient --> DECLINED
                default: return itemGlyphNeutral;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case itemGlyphCheck: return "ACCEPTED";
                case itemGlyphPlus: return "ACCEPTING";
                //case itemGlyphPlus: return "PENDING";
                case itemGlyphX: return "DECLINED";
                case itemGlyphMinus: return "DECLINING";
                default: return "";
            }
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
