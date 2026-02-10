using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MatsedelnShared.Models
{
    // Plain POCO - UI change notification is handled by MenuWrapper in the WPF client.
    // This keeps the shared library free of UI framework dependencies.
    public class MenuEntry
    {
        private Recipe? _lunchRecipe;
        private Recipe? _dinnerRecipe;

        // Setters sync the FK ids to keep model consistent without requiring ObservableObject.
        public Recipe? LunchRecipe
        {
            get => _lunchRecipe;
            set
            {
                _lunchRecipe = value;
                LunchRecipeId = value?.Id;
            }
        }

        public Recipe? DinnerRecipe
        {
            get => _dinnerRecipe;
            set
            {
                _dinnerRecipe = value;
                DinnerRecipeId = value?.Id;
            }
        }

        public int? LunchRecipeId { get; set; }  // FK to Recipe
        public int? DinnerRecipeId { get; set; }  // FK to Recipe

        [Key]
        public int Id { get; set; } = 0;

        [Required]
        public DateTime Date { get; set; }

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
