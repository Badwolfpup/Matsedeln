using CommunityToolkit.Mvvm.ComponentModel;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Wrappers
{
    public partial class RecipeWrapper: ObservableObject
    {
        public Recipe recipe { get; set; } // Reference to actual model

        public ObservableCollection<Ingredient> Ingredientlist { get; set; }
        [ObservableProperty]
        private bool _isHighlighted;
        [ObservableProperty]
        private bool _isSelected;

        public RecipeWrapper()
        {
            Ingredientlist = new ObservableCollection<Ingredient>();
            recipe = new Recipe();
        }
    }
}
