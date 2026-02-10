using System.ComponentModel.DataAnnotations;

namespace MatsedelnShared.Models
{
    // Plain POCO - UI change notification is handled by RecipeWrapper in the WPF client.
    // This keeps the shared library free of UI framework dependencies.
    public class Recipe
    {
        public Recipe(string name)
        {
            Name = name;
        }

        public Recipe()
        {
        }

        public Recipe(Recipe recipe)
        {
            Name = recipe.Name;
            Id = recipe.Id;
            ImagePath = recipe.ImagePath;
            IsDish = recipe.IsDish;
            Ingredientlist = new List<Ingredient>(recipe.Ingredientlist);
            ChildRecipes = new List<RecipeHierarchy>(recipe.ChildRecipes);
        }

        [Key]
        public int Id { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string? ImagePath { get; set; } = "pack://application:,,,/Images/dummybild.png";

        public bool IsDish { get; set; } = false;

        // EF Core and JSON prefer ICollection or List for the database layer
        public virtual ICollection<Ingredient> Ingredientlist { get; set; } = new List<Ingredient>();
        public virtual ICollection<RecipeHierarchy> ChildRecipes { get; set; } = new List<RecipeHierarchy>();
        public virtual ICollection<RecipeHierarchy> ParentRecipes { get; set; } = new List<RecipeHierarchy>();
    }
}
