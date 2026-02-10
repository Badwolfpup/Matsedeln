using System.Globalization;
using System.Windows.Data;

namespace Matsedeln.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrWhiteSpace(imagePath))
            {
                // --- CONFIGURATION ---
                string baseUrl;

#if DEBUG
                baseUrl = "https://localhost:7039";
#else
            baseUrl = "https://your-smarterasp-domain.com"; 
#endif
                // Ensure we don't end up with double slashes (e.g., https://site.com//images/test.png)
                return $"{baseUrl.TrimEnd('/')}/{imagePath.TrimStart('/')}";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
