using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace MatsedelnShared.Models
{
    public partial class MenuEntry: ObservableObject
    {

        [ObservableProperty]
        private Recipe? lunchRecipe;

        [ObservableProperty]
        private Recipe? dinnerRecipe;

       
        public int? LunchRecipeId { get; set; }  // FK to Recipe
        public int? DinnerRecipeId { get; set; }  // FK to Recipe

        [Key]
        public int Id { get; set; } = 0;

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

        partial void OnLunchRecipeChanged(Recipe? value) => LunchRecipeId = value?.Id;


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
