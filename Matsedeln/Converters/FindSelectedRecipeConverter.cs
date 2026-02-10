using MatsedelnShared.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Matsedeln.Usercontrols
{
    public class FindSelectedRecipeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var menuEntry = values[0] as MenuEntry;
            var recipelist = values[1] as ObservableCollection<Recipe>;
            var lunchOrDinner = values[2] as string;
            if (menuEntry == null || recipelist == null || string.IsNullOrEmpty(lunchOrDinner)) return false;
            if (lunchOrDinner == "lunch")
            {
                if (menuEntry.LunchRecipeId == null) return recipelist[0];
                var recipe = recipelist.FirstOrDefault(r => r.Id == menuEntry.LunchRecipeId);
                return recipe ?? recipelist[0];

            }
            else if (lunchOrDinner == "dinner")
            {
                if (menuEntry.DinnerRecipeId == null) return recipelist[0];
                var recipe = recipelist.FirstOrDefault(r => r.Id == menuEntry.DinnerRecipeId);
                return recipe ?? recipelist[0];

            }
            return recipelist[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
