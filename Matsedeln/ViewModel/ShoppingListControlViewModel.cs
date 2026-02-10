using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Matsedeln.ViewModel
{
    public partial class ShoppingListViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [ObservableProperty]
        private ObservableCollection<Ingredient> shoppingList;

        [ObservableProperty]
        private bool showShoppingList = true;


        public ShoppingListViewModel()
        {
            ShoppingList = new ObservableCollection<Ingredient>();
            WeakReferenceMessenger.Default.Register<Messengers.UpdateShoppingListMessenger>(this, (r, m) =>
            {
                if (m.AddorRemove)
                {
                    AddIngredientToShoppinglist(m.wrapper);
                }
                else
                {
                    RemoveIngredientsFromShoppinglist(m.wrapper);
                }
            });
        }


        [RelayCommand]
        private void AbortIngredientlist()
        {

        }
        [RelayCommand]
        private void CopyShoppinglist()
        {
            string result = "";

            foreach (var item in ShoppingList)
            {
                result += item.ToString() + "\n";
            }
            //CopyList();
            Clipboard.SetText(string.Join(Environment.NewLine, ShoppingList.Select(x => x.ToString())));
        }

        [RelayCommand]
        private void CopyList()
        {
            ShowShoppingList = false;
        }

        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ShoppingList.Contains(ingredient)) ShoppingList.Remove(ingredient);
        }

        private async void AddIngredientToShoppinglist(RecipeWrapper wrapper)
        {
            try
            {
                var api = ApiService.Instance;
                var hierarchty = await api.GetListAsync<RecipeHierarchy>("api/RecipeHierarchy");
                var quantity = hierarchty?.FirstOrDefault(x => x.ParentRecipeId == wrapper.Recipe.Id)?.Quantity ?? 1;
                var ingredients = wrapper.Recipe.ChildRecipes.SelectMany(x => x.ChildRecipe.Ingredientlist).ToList() ?? new List<Ingredient>();
                ingredients = Enumerable.Repeat(ingredients, quantity).SelectMany(x => x).ToList();
                foreach (var item in wrapper.Recipe.Ingredientlist)
                {
                    ingredients.Add(item);
                }
                ingredients.ForEach(ingredient =>
                {
                    if (!ShoppingList.Any(i => i.Good.Name == ingredient.Good.Name))
                    {
                        ShoppingList.Add(new Ingredient(ingredient));
                    }
                    else
                    {
                        var existingItem = ShoppingList.First(i => i.Good.Name == ingredient.Good.Name);
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
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new Messengers.ToastMessage("Kunde inte hämta ingredienser: " + ex.Message));
            }
        }

        private async void RemoveIngredientsFromShoppinglist(RecipeWrapper wrapper)
        {
            try
            {
                var api = ApiService.Instance;
                var hierarchty = await api.GetListAsync<RecipeHierarchy>("api/RecipeHierarchy");
                var quantity = hierarchty?.FirstOrDefault(x => x.ParentRecipeId == wrapper.Recipe.Id)?.Quantity ?? 1;
                var ingredients = wrapper.Recipe.ChildRecipes.SelectMany(x => x.ChildRecipe.Ingredientlist).ToList() ?? new List<Ingredient>();
                ingredients = Enumerable.Repeat(ingredients, quantity).SelectMany(x => x).ToList();
                foreach (var item in wrapper.Recipe.Ingredientlist)
                {
                    ingredients.Add(item);
                }
                ingredients.ForEach(ingredient =>
                {
                    if (!ShoppingList.Any(i => i.Good.Name == ingredient.Good.Name))
                    {
                        ShoppingList.Remove(ingredient);
                    }
                    else
                    {
                        var existingItem = ShoppingList.First(i => i.Good.Name == ingredient.Good.Name);
                        existingItem.QuantityInGram -= ingredient.QuantityInGram;
                        existingItem.QuantityInDl -= ingredient.QuantityInDl;
                        existingItem.QuantityInSt -= ingredient.QuantityInSt;
                        existingItem.QuantityInMsk -= ingredient.QuantityInMsk;
                        existingItem.QuantityInTsk -= ingredient.QuantityInTsk;
                        existingItem.Quantity -= ingredient.GetQuantity(existingItem);
                        existingItem.ConvertToOtherUnits();
                        if (existingItem.Quantity <= 0) ShoppingList.Remove(existingItem);
                    }
                });
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new Messengers.ToastMessage("Kunde inte ta bort ingredienser: " + ex.Message));
            }
        }


        //private void CopyList()
        //{
        //    int qmax = ShoppingList.Max(x => x.Quantity).ToString().Length;
        //    int umax = ShoppingList.Max(y => y.Unit.Length);
        //    if (qmax == 0 || umax == 0) return;
        //    Clipboard.SetText(string.Join(Environment.NewLine, ShoppingList.Select(x =>
        //    {
        //        int qlength = x.Quantity.ToString().Length;
        //        int ulength = x.Unit.Length;
        //        string quantity = $"{(qlength < qmax ? string.Concat(Enumerable.Repeat(" ", qmax - qlength)) : "")}{x.Quantity}";
        //        string unit = $"{x.Unit}{(ulength < umax ? string.Concat(Enumerable.Repeat(" ", umax - ulength)) : "")}";
        //        return $"{quantity} {unit} {x.Good.Name}";
        //    })));

        //}

        [RelayCommand]
        private void UnitComboBox(Ingredient ingredient)
        {
            if (ingredient == null) return;

            if (ingredient.Unit == "g") { ingredient.Quantity = ingredient.QuantityInGram; }
            else if (ingredient.Unit == "dl") { ingredient.Quantity = ingredient.QuantityInDl; }
            else if (ingredient.Unit == "msk") { ingredient.Quantity = ingredient.QuantityInMsk; }
            else if (ingredient.Unit == "tsk") { ingredient.Quantity = ingredient.QuantityInTsk; }
            else if (ingredient.Unit == "krm") { ingredient.Quantity = ingredient.QuantityInKrm; }
            else if (ingredient.Unit == "st") { ingredient.Quantity = ingredient.QuantityInSt; }

        }

    }


}
