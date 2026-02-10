using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Matsedeln.Usercontrols
{
    public class ShowAddIngredientBtnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool show) return Visibility.Visible;
            return show ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
