using Matsedeln.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Matsedeln
{
    public class MenuItem: INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private Recipe recipe;

        public Recipe Recipe
        {
            get { return recipe; }
            set
            {
                if (recipe != value)
                {
                    recipe = value;
                    OnPropertyChanged(nameof(recipe));
                }
            }
        }

        public  DateTime Date { get; set; }

        public MenuItem(Recipe recipe, DateTime date)
        {
            Recipe = recipe;
            Date = date;
        }

        public override string ToString()
        {
            CultureInfo culture = new CultureInfo("sv-SE");
            string day = Date.ToString("ddd", culture);
            string date = Date.ToString("d/M", CultureInfo.InvariantCulture);
            return $"{day} {date}";
        }
    }
}
