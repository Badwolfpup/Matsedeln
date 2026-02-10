using CommunityToolkit.Mvvm.ComponentModel;
using Matsedeln.Pages;
using MatsedelnShared.Models;
using System.Globalization;

namespace Matsedeln.Wrappers
{
    // Wraps MenuEntry to add UI-specific state (IsLunchHighlighted, IsDinnerHighlighted)
    // and PropertyChanged notification. The MenuEntry model itself is a plain POCO
    // kept in the shared library without UI framework dependencies.
    public partial class MenuWrapper : ObservableObject
    {
        private readonly MenuEntry _menu;

        // Exposes the underlying model for API calls and data operations.
        public MenuEntry Menu => _menu;

        // Proxy properties that delegate to the model and raise PropertyChanged.
        public int Id
        {
            get => _menu.Id;
            set => SetProperty(_menu.Id, value, _menu, (m, v) => m.Id = v);
        }

        public DateTime Date => _menu.Date;

        public int? LunchRecipeId
        {
            get => _menu.LunchRecipeId;
            set => SetProperty(_menu.LunchRecipeId, value, _menu, (m, v) => m.LunchRecipeId = v);
        }

        public int? DinnerRecipeId
        {
            get => _menu.DinnerRecipeId;
            set => SetProperty(_menu.DinnerRecipeId, value, _menu, (m, v) => m.DinnerRecipeId = v);
        }

        public Recipe? LunchRecipe
        {
            get => _menu.LunchRecipe;
            set
            {
                if (SetProperty(_menu.LunchRecipe, value, _menu, (m, v) => m.LunchRecipe = v))
                {
                    OnPropertyChanged(nameof(LunchRecipeId));
                    OnPropertyChanged(nameof(LunchImagePath));
                }
            }
        }

        public Recipe? DinnerRecipe
        {
            get => _menu.DinnerRecipe;
            set
            {
                if (SetProperty(_menu.DinnerRecipe, value, _menu, (m, v) => m.DinnerRecipe = v))
                {
                    OnPropertyChanged(nameof(DinnerRecipeId));
                    OnPropertyChanged(nameof(DinnerImagePath));
                }
            }
        }

        public string LunchImagePath => _menu.LunchRecipe?.ImagePath ?? "";
        public string DinnerImagePath => _menu.DinnerRecipe?.ImagePath ?? "";

        [ObservableProperty]
        private bool _isLunchHighlighted;

        [ObservableProperty]
        private bool _isDinnerHighlighted;

        public MenuWrapper(MenuEntry menu)
        {
            _menu = menu;
        }

        public MenuWrapper() : this(new MenuEntry())
        {
        }

        public override string ToString()
        {
            DateConverter dc = new DateConverter();
            var converted = dc.Convert(Date, typeof(string), true, CultureInfo.CurrentCulture);
            return converted?.ToString() ?? "";
        }
    }
}
