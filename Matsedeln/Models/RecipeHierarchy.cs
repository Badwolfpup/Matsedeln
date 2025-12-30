using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Matsedeln.Models
{
    public class RecipeHierarchy
    {
        public RecipeHierarchy()
        {
        }

        public RecipeHierarchy(Recipe parent, Recipe child)
        {
            ParentRecipe = parent;
            ChildRecipe = child;
            ParentRecipeId = parent.Id;
            ChildRecipeId = child.Id;
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ParentRecipe")]
        public int ParentRecipeId { get; set; }
        public Recipe ParentRecipe { get; set; }

        [ForeignKey("ChildRecipe")]
        public int ChildRecipeId { get; set; }
        public Recipe ChildRecipe { get; set; }

        
    }
}

