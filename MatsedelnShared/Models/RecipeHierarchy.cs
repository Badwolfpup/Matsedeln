using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatsedelnShared.Models
{
    public class RecipeHierarchy
    {
        public RecipeHierarchy()
        {
        }

        public RecipeHierarchy(Recipe parent, Recipe child, int quantity = 1)
        {
            ParentRecipe = parent;
            ChildRecipe = child;
            ParentRecipeId = parent.Id;
            ChildRecipeId = child.Id;
            Quantity = quantity;
        }

        [Key]
        public int Id { get; set; } = 0;

        [ForeignKey("ParentRecipe")]
        public int ParentRecipeId { get; set; }
        public virtual Recipe? ParentRecipe { get; set; }

        [ForeignKey("ChildRecipe")]
        public int ChildRecipeId { get; set; }
        public virtual Recipe? ChildRecipe { get; set; }

        public int Quantity { get; set; } = 1;

    }
}

