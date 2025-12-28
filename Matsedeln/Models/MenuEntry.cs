using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Matsedeln.Models
{
    public class MenuEntry: INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
     
        public AppData Ad { get; } = AppData.Instance;

        private Recipe? lunchRecipe;

        public Recipe? LunchRecipe
        {
            get { return lunchRecipe; }
            set
            {
                if (lunchRecipe != value)
                {
                    lunchRecipe = value;
                    LunchRecipeId = value?.Id;
                    OnPropertyChanged(nameof(LunchRecipe));
                }
            }
        }

        private Recipe? dinnerRecipe;

        public Recipe? DinnerRecipe
        {
            get { return dinnerRecipe; }
            set
            {
                if (dinnerRecipe != value)
                {
                    dinnerRecipe = value;
                    DinnerRecipeId = value?.Id;
                    OnPropertyChanged(nameof(DinnerRecipe));
                }
            }
        }


        public int? LunchRecipeId { get; set; }  // FK to Recipe
        public int? DinnerRecipeId { get; set; }  // FK to Recipe

        [Key]
        public int Id { get; set; }

        [Required]
        public  DateTime Date { get; set; }



        public MenuEntry(Recipe lunchrecipe, Recipe dinnerrecipe, DateTime date)
        {
            LunchRecipe = lunchrecipe;
            DinnerRecipe = dinnerrecipe;
            Date = date;
        }

        public MenuEntry()
        {
        }

        public MenuEntry(MenuEntry menu)
        {
            LunchRecipe = menu.LunchRecipe;
            DinnerRecipe = menu.DinnerRecipe;
            LunchRecipeId = menu.LunchRecipeId;
            DinnerRecipeId = menu.DinnerRecipeId;
            Id = menu.Id;
            Date = menu.Date;
        }

        public override string ToString()
        {
            CultureInfo culture = new CultureInfo("sv-SE");
            string day = Date.ToString("dddd", culture);
            string capitalized = culture.TextInfo.ToTitleCase(day);
            string date = Date.ToString("d/M", CultureInfo.InvariantCulture);
            return $"{capitalized} {date}";
        }
    }
}
