using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Matsedeln.Pages
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                var weekday = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(date.ToString("ddd").ToLower());
                var datum = date.ToString("d/M", CultureInfo.InvariantCulture);
                return $"{weekday} {datum}";
            }
            return ((DateTime)value).ToString("ddd d/M");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
