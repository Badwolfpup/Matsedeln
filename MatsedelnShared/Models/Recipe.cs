using CommunityToolkit.Mvvm.ComponentModel;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MatsedelnShared.Models
{
    public partial class Recipe : ObservableObject
    {
        public Recipe(string name)
        {
            this.name = name;
            Ingredientlist = new ObservableCollection<Ingredient>(Ingredientlist);
        }

        public Recipe()
        {
        }

        public Recipe(Recipe recipe)
        {
            this.name = recipe.Name;
            this.Id = recipe.Id;
            this.imagePath = recipe.ImagePath;
            this.Ingredientlist = new ObservableCollection<Ingredient>(recipe.Ingredientlist);
            this.ParentRecipes = recipe.ParentRecipes;
            this.ChildRecipes = recipe.ChildRecipes;
        }
        [Key]
        public int Id { get; set; } = 0;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string? imagePath = "pack://application:,,,/Images/dummybild.png";

        // EF Core and JSON prefer ICollection or List for the database layer
        public virtual ICollection<Ingredient> Ingredientlist { get; set; } = new List<Ingredient>();
        public virtual ICollection<RecipeHierarchy> ChildRecipes { get; set; } = new List<RecipeHierarchy>();
        public virtual ICollection<RecipeHierarchy> ParentRecipes { get; set; } = new List<RecipeHierarchy>();


    }
}
