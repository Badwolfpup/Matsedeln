using CommunityToolkit.Mvvm.ComponentModel;
using MatsedelnShared.Models;
using System.Collections.ObjectModel;

namespace Matsedeln.Wrappers
{
    // Wraps Recipe to add UI-specific state (IsHighlighted, IsSelected) and
    // PropertyChanged notification. The Recipe model itself is a plain POCO
    // kept in the shared library without UI framework dependencies.
    public partial class RecipeWrapper : ObservableObject
    {
        private readonly Recipe _recipe;

        // Exposes the underlying model for API calls and data operations.
        public Recipe Recipe => _recipe;

        // Proxy properties that delegate to the model and raise PropertyChanged
        // so WPF bindings update when values change.
        public int Id
        {
            get => _recipe.Id;
            set => SetProperty(_recipe.Id, value, _recipe, (r, v) => r.Id = v);
        }

        public string Name
        {
            get => _recipe.Name;
            set => SetProperty(_recipe.Name, value, _recipe, (r, v) => r.Name = v);
        }

        public string? ImagePath
        {
            get => _recipe.ImagePath;
            set => SetProperty(_recipe.ImagePath, value, _recipe, (r, v) => r.ImagePath = v);
        }

        public bool IsDish
        {
            get => _recipe.IsDish;
            set => SetProperty(_recipe.IsDish, value, _recipe, (r, v) => r.IsDish = v);
        }

        // ObservableCollections that keep the model's ICollections in sync.
        // XAML binds to these for live collection updates.
        public ObservableCollection<RecipeHierarchy> ChildRecipes { get; set; }
        public ObservableCollection<Ingredient> Ingredientlist { get; set; }

        [ObservableProperty]
        private bool _isHighlighted;
        [ObservableProperty]
        private bool _isSelected;

        public RecipeWrapper(Recipe recipe)
        {
            _recipe = recipe;
            Ingredientlist = new ObservableCollection<Ingredient>(recipe.Ingredientlist);
            ChildRecipes = new ObservableCollection<RecipeHierarchy>(recipe.ChildRecipes);
            // Sync back: the model's collections point to these ObservableCollections
            // so additions/removals in UI are reflected in the model.
            _recipe.Ingredientlist = Ingredientlist;
            _recipe.ChildRecipes = ChildRecipes;
        }

        public RecipeWrapper() : this(new Recipe())
        {
        }
    }
}
