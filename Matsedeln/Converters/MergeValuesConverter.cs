using System.Globalization;
using System.Windows.Data;

namespace Matsedeln.Usercontrols
{
    public class MergeValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone(); // Returns object[] with both values
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
