using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Matsedeln.Usercontrols
{
    public class ToSweDecimalConverter : IValueConverter
    {
        private static readonly CultureInfo SwedishCulture = new CultureInfo("sv-SE");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text) return text;
            if (value is double doubleValue)
            {
                // Format with Swedish culture (comma separator)
                string formatted = doubleValue.ToString("F2", SwedishCulture);
                // Trim trailing zeros and comma if whole number (e.g., "1,00" -> "1")
                if (formatted.EndsWith(",00")) return formatted.Substring(0, formatted.Length - 3);
                if (formatted.EndsWith("0")) return formatted.TrimEnd('0').TrimEnd(',');  // e.g., "1,50" -> "1,5"
                return formatted;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (stringValue.EndsWith(",") || stringValue.EndsWith(".")) return DependencyProperty.UnsetValue;

                // Try Swedish first
                if (double.TryParse(stringValue, NumberStyles.Any, SwedishCulture, out var result))
                {
                    return result;
                }
                // Fallback to invariant (handles "1.5")
                if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}