using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Matsedeln.ViewModel
{
    public partial class RecipePageViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;
        [ObservableProperty]
        private CollectionViewSource recipesViewSource;

        [ObservableProperty]
        ObservableCollection<RecipeWrapper> recipeList;

        private ShoppingListControl shoppinglist;

        [ObservableProperty]
        private string filterText = string.Empty;

        public RecipePageViewModel()
        {
            Ad.ShoppingList.Clear();
            WeakReferenceMessenger.Default.Register<NameExistsMessenger>(this, async (r, m) => { if (RecipeList != null && RecipeList.Count > 0) m.Reply(RecipeList.Any(x => x.recipe.Name == m.Name)); });
        }

        [RelayCommand]
        private async Task LoadRecipe()
        {
            var api = new ApiService();
            var list = await api.GetListAsync<Recipe>("api/recipe");
            if (list == null) { RecipeList = new ObservableCollection<RecipeWrapper>(); }
            RecipeList = new ObservableCollection<RecipeWrapper>(list?.Select(x => new RecipeWrapper { recipe = x }) ?? Enumerable.Empty<RecipeWrapper>());
            SetSource();

        }

        private void SetSource()
        {
            RecipesViewSource = new CollectionViewSource();
            RecipesViewSource.Source = RecipeList;
            RecipesViewSource.View.Culture = new CultureInfo("sv-SE");
            RecipesViewSource.SortDescriptions.Add(new SortDescription("recipe.Name", ListSortDirection.Ascending));
            RecipesViewSource.Filter += FilterRecipe;
        }

        private void FilterRecipe(object sender, FilterEventArgs e)
        {
            if (e.Item is RecipeWrapper wrapper)
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = wrapper.recipe.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 || wrapper.IsSelected;
                }
            }
        }




        [RelayCommand]
        private void EditRecipe(Recipe recipe)
        {
            WeakReferenceMessenger.Default.Send(new SelectedRecipeMessenger(recipe));
        }

        [RelayCommand]
        private async Task DeleteRecipe()
        {
            var sum = RecipeList.Sum(x => x.IsHighlighted ? 1 : 0);
            if (sum == 0)
            {
                MessageBox.Show("Du måste välja något innan du kan radera.");
                return;
            }
            else if (sum > 1)
            {
                MessageBox.Show("Du får bara välja ett recept innan du kan radera.");
                return;
            }
            var SelectedRecipe = RecipeList.FirstOrDefault(x => x.IsHighlighted);
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Det gick inte att radera. Problem att hitta datacontext");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Vill du verkligen radera detta recept?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var api = new ApiService();
                if (!await api.DeleteAsync($"api/recipe/{SelectedRecipe.recipe.Id}"))
                {
                    MessageBox.Show("Det gick inte att radera receptet");
                    return;
                }
                WeakReferenceMessenger.Default.Send(new ToastMessage("Receptet har raderats."));
                RecipeList.Remove(SelectedRecipe);
                RecipesViewSource.View.Refresh();
            }
        }

        [RelayCommand]
        public void ShowMenuPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("menu"));
        }

        [RelayCommand]
        public void ShowIngredientsPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("goods"));
        }

        [RelayCommand]
        private void SelectBorder(GoodsWrapper wrapper)
        {
            if (wrapper == null) return;
            wrapper.IsHighlighted = !wrapper.IsHighlighted;
            WeakReferenceMessenger.Default.Send(new SelectedGoodsMessenger(wrapper));
        }

    }
}
