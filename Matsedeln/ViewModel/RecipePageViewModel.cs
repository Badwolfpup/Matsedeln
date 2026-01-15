using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Matsedeln.ViewModel
{
    public partial class RecipePageViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;
        public CollectionViewSource RecipesViewSource { get; set; }

        private ShoppingListControl shoppinglist;


        public RecipePageViewModel()
        {
            Ad.ShoppingList.Clear();
            RecipesViewSource = new CollectionViewSource();
            RecipesViewSource.Source = Ad.RecipesList;
            RecipesViewSource.Filter += FilterRecipe;
            WeakReferenceMessenger.Default.Register<AppData.RefreshCollectionViewMessage>(this, (r, m) => RecipesViewSource.View.Refresh());
            WeakReferenceMessenger.Default.Register<AppData.AddIngredientShopListMessage>(this, (r, m) => AddIngredientToShoppinglist(m.recipe));
            WeakReferenceMessenger.Default.Register<AppData.RemoveIngredientShoplistMessage>(this, (r, m) => RemoveIngredientsFromShoppinglist(m.recipe));
            WeakReferenceMessenger.Default.Register<AppData.GoBackToShoppingListMessage>(this, (r, m) => Ad.CurrentUserControl = shoppinglist);
        }

        private void FilterRecipe(object sender, FilterEventArgs e)
        {
            if (e.Item is Recipe recipe)
            {
                if (!string.IsNullOrEmpty(Ad.FilterText))
                {
                    if (recipe.Name.IndexOf(Ad.FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = true;
                }
            }
        }
        public void OnPageLoaded()
        {
            RecipesViewSource.View.Refresh();
        }

        private void AddIngredientToShoppinglist(Recipe recipe)
        {
            if (Ad.CurrentUserControl is ShoppingListControl shop && shop.ShowShoppinglist) return;

            var ingredients = recipe.ChildRecipes.SelectMany(x => x.ChildRecipe.Ingredientlist).ToList();
            if (ingredients == null) ingredients = new List<Ingredient>();
            foreach (var item in recipe.Ingredientlist)
            {
                ingredients.Add(item);
            }
            ingredients.ForEach(ingredient =>
            {
                if (!Ad.ShoppingList.Any(i => i.Good.Name == ingredient.Good.Name))
                {
                    Ad.ShoppingList.Add(new Ingredient(ingredient));
                }
                else
                {
                    var existingItem = Ad.ShoppingList.First(i => i.Good.Name == ingredient.Good.Name);
                    existingItem.QuantityInGram += ingredient.QuantityInGram;
                    existingItem.QuantityInDl += ingredient.QuantityInDl;
                    existingItem.QuantityInSt += ingredient.QuantityInSt;
                    existingItem.QuantityInMsk += ingredient.QuantityInMsk;
                    existingItem.QuantityInTsk += ingredient.QuantityInTsk;
                    existingItem.Quantity += ingredient.GetQuantity(existingItem);
                    existingItem.ConvertToOtherUnits();
                }
            });
        }

        [RelayCommand]
        private void EditRecipe(Recipe recipe)
        {
            shoppinglist = Ad.CurrentUserControl as ShoppingListControl ?? new ShoppingListControl();
            Ad.CurrentUserControl = new NewRecipeControl(recipe);
        }


        private void RemoveIngredientsFromShoppinglist(Recipe recipe)
        {
            if (Ad.CurrentUserControl is ShoppingListControl shop && shop.ShowShoppinglist) return;

            recipe.Ingredientlist.ToList().ForEach(ingredient =>
            {
                var itemToRemove = Ad.ShoppingList.FirstOrDefault(i => i.Good.Name == ingredient.Good.Name);
                if (itemToRemove != null)
                {
                    if (itemToRemove.QuantityInGram == ingredient.QuantityInGram) Ad.ShoppingList.Remove(itemToRemove);
                    else
                    {
                        itemToRemove.QuantityInGram -= ingredient.QuantityInGram;
                        itemToRemove.QuantityInDl -= ingredient.QuantityInDl;
                        itemToRemove.QuantityInSt -= ingredient.QuantityInSt;
                        itemToRemove.QuantityInMsk -= ingredient.QuantityInMsk;
                        itemToRemove.QuantityInTsk -= ingredient.QuantityInTsk;
                        itemToRemove.Quantity -= ingredient.GetQuantity(itemToRemove);
                        itemToRemove.ConvertToOtherUnits();
                    }
                }
            });
        }


    }
}
