/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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
