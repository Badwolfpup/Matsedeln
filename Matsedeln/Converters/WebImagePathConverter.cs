using System.Globalization;
using System.Windows.Data;

namespace Matsedeln.Converters
{
    public class WebImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url;
            if (value is string imagePath && !string.IsNullOrWhiteSpace(imagePath))
            {
                string baseUrl =
#if DEBUG
                    "https://localhost:7039";
#else
            "https://your-smarterasp-domain.com"; 
#endif
                url = $"{baseUrl.TrimEnd('/')}/{imagePath.TrimStart('/')}";
            }
            else
            {
                url = "pack://application:,,,/Images/dummybild.png";
            }

            // Explicitly return a Uri or BitmapImage to satisfy WPF's Image.Source
            try
            {
                return new System.Windows.Media.Imaging.BitmapImage(new Uri(url));
            }
            catch
            {
                // If the URL is malformed, fall back to the dummy image
                return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}