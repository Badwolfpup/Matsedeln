using System.Globalization;
using System.Windows.Data;

namespace Matsedeln.Pages
{
    public class GoodsnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var text = value as string;
            //if (string.IsNullOrWhiteSpace(text)) return value;

            //var words = text.Split(' ');
            //if (words.Length < 2) return words[0].Length > 20 ? words[0].Substring(0, 20) : words[0];

            //var firstWord = words[0].Length > 20 ? words[0].Substring(0, 20) : words[0];
            //var secondWord = words[1].Length > 20 ? words[1].Substring(0, 20) : words[1];
            //return firstWord + "\r\n" + secondWord;

            string name = value as string;
            if (string.IsNullOrEmpty(name)) return value;
            return name.Length < 20 ? name : name.Substring(0, 16) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
