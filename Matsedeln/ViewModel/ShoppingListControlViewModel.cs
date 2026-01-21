using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Usercontrols;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Matsedeln.ViewModel
{
    public partial class ShoppingListViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;


        public ShoppingListViewModel()
        {
            WeakReferenceMessenger.Default.Register<AppData.ResetShoppinglistUCMessages>(this, (r, m) => AbortIngredientlist());
        }

 
        [RelayCommand]
        private void AbortIngredientlist()
        {
            Ad.ShoppingList.Clear();
            //var request = new FindBorderShopListMessage(Ad.RecipePageInstance.RecipeItemsControl);
            //var response = WeakReferenceMessenger.Default.Send(request);
            //var allBorders = response.Response;
            WeakReferenceMessenger.Default.Send(new AppData.RemoveHighlightRecipeMessage());
            WeakReferenceMessenger.Default.Send(new AppData.ShowShoppingListMessage());
            //foreach (var item in allBorders)
            //{
            //    if (item is Border border)  border.BorderBrush = System.Windows.Media.Brushes.Transparent;
            //}
        }
        [RelayCommand]
        private void CopyShoppinglist()
        {
            string result = "";

            foreach (var item in Ad.ShoppingList)
            {
                result += item.ToString() + "\n";
            }
            //CopyList();
            Clipboard.SetText(string.Join(Environment.NewLine, Ad.ShoppingList.Select(x => x.ToString())));
        }

        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (Ad.ShoppingList.Contains(ingredient)) Ad.ShoppingList.Remove(ingredient);
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


        private void CopyList()
        {
            int qmax = Ad.ShoppingList.Max(x => x.Quantity).ToString().Length;
            int umax = Ad.ShoppingList.Max(y => y.Unit.Length);
            if (qmax == 0 || umax == 0) return;
            Clipboard.SetText(string.Join(Environment.NewLine, Ad.ShoppingList.Select(x =>
            {
                int qlength = x.Quantity.ToString().Length;
                int ulength = x.Unit.Length;
                string quantity = $"{(qlength < qmax ? string.Concat(Enumerable.Repeat(" ", qmax - qlength)) : "")}{x.Quantity}";
                string unit = $"{x.Unit}{(ulength < umax ? string.Concat(Enumerable.Repeat(" ", umax - ulength)) : "")}";
                return $"{quantity} {unit} {x.Good.Name}";
            })));

        }
        [RelayCommand]
        private void UnitComboBox(Ingredient ingredient)
        {
            if (ingredient == null) return;

            if (ingredient.Unit == "g") {  ingredient.Quantity = ingredient.QuantityInGram; }
            else if (ingredient.Unit == "dl") { ingredient.Quantity = ingredient.QuantityInDl; }
            else if (ingredient.Unit == "msk") { ingredient.Quantity = ingredient.QuantityInMsk; }
            else if (ingredient.Unit == "tsk") { ingredient.Quantity = ingredient.QuantityInTsk; }
            else if (ingredient.Unit == "krm") { ingredient.Quantity = ingredient.QuantityInKrm; }
            else if (ingredient.Unit == "st") { ingredient.Quantity = ingredient.QuantityInSt; }

        }

    }


}
