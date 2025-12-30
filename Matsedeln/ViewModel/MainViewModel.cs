using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Matsedeln
{
    public partial class MainViewModel: ObservableObject
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public AppData Ad { get; } = AppData.Instance;


        #region Properties
        [ObservableProperty]
        private bool showGoodsUsercontrol = false;
        [ObservableProperty]
        private bool showRecipeUsercontrol = false;
        private Goods selectedGood;
        #endregion

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<AppData.PassGoodsToUCMessage>(this, (r, m) => selectedGood = m.good);
        }

        [RelayCommand]
        private void ShowRecipesPage()
        {
            Ad.CurrentPage = Ad.RecipePageInstance;
            Ad.CurrentUserControl = new ShoppingListControl();
            Ad.FilterText = string.Empty;
            Ad.IsFilterTextboxEnabled = false;
            WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
        }

        [RelayCommand]
        private void ShowIngredientsPage()
        {
            Ad.CurrentPage = Ad.IngredientPageInstance;
            Ad.CurrentUserControl = new NewGoodsControl();
            Ad.FilterText = string.Empty;
            Ad.IsFilterTextboxEnabled = true;
            if (Ad.CurrentUserControl is ShoppingListControl shop)
            {
                shop.ShowIngredients = false;
                shop.ShowShoppinglist = false;
            }
            WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
        }

        [RelayCommand]
        private void ShowMenuPage()
        {
            Ad.CurrentPage = Ad.MenuPageInstance;
            Ad.CurrentUserControl = new WeeklyMenuControl();
            WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
        }

        [RelayCommand]
        private void FilterGoods()
        {
            WeakReferenceMessenger.Default.Send(new AppData.RefreshCollectionViewMessage());
        }


        [RelayCommand]
        private async void DeleteGoodOrRecipe(object sender)
        {
            if (Ad.CurrentPage is IngredientPage)
            {
                if (selectedGood == null)
                {
                    MessageBox.Show("Du måste välja något innan du kan radera.");
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Vill du verkligen radera denna vara?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (!await Ad.GoodsService.DeleteGoods(selectedGood, Ad.GoodsList))
                    {
                        MessageBox.Show("Det gick inte att radera varan");
                        return;
                    }
                    Ad.GoodsList.Remove(selectedGood);
                    var goodtoremove = Ad.RecipesList.Where(x => x.Ingredientlist.Any(y => y.GoodsId == selectedGood.Id)).Select(y => y.Ingredientlist).ToList();
                    foreach (var item in goodtoremove)
                    {
                        var ing = item.FirstOrDefault(x => x.GoodsId == selectedGood.Id);

                        item.Remove(ing);
                    }
                    WeakReferenceMessenger.Default.Send(new AppData.RefreshCollectionViewMessage());
                    WeakReferenceMessenger.Default.Send(new AppData.PassGoodsToUCMessage(new Goods()));
                }
            }
            else
            {
                var allBorders = FindAllBorders(Ad.RecipePageInstance.RecipeItemsControl);
                var sum = allBorders.Sum(x => x.BorderBrush == Brushes.Red ? 1 : 0);
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
                var SelectedBorder = allBorders.FirstOrDefault(x => x.DataContext is Recipe && x.BorderBrush == Brushes.Red);
                if (SelectedBorder is Border b && b.DataContext is Recipe SelectedRecipe)
                {
                    if (SelectedRecipe == null)
                    {
                        MessageBox.Show("Det gick inte att radera. Problem att hitta datacontext");
                        return;
                    }
                    MessageBoxResult result = MessageBox.Show("Vill du verkligen radera detta recept?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (!await Ad.RecipeService.DeleteRecipe(SelectedRecipe, Ad.RecipesList))
                        {
                            MessageBox.Show("Det gick inte att radera receptet");
                            return;
                        }

                        Ad.RecipesList.Remove(SelectedRecipe);
                        foreach (var item in Ad.MenuList)
                        {
                            if (item.LunchRecipeId == SelectedRecipe.Id) item.LunchRecipe = null;
                            if (item.DinnerRecipeId == SelectedRecipe.Id) item.DinnerRecipe = null;
                        }
                        WeakReferenceMessenger.Default.Send(new AppData.RefreshMenuEntrySourceMessage());
                        WeakReferenceMessenger.Default.Send(new AppData.ResetShoppinglistUCMessages());
                        ((RecipePageViewModel)Ad.RecipePageInstance.DataContext).RecipesViewSource.View.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Det gick inte att radera. Problem att hitta border");
                    return;
                }
            }
        }

        public IEnumerable<Border> FindAllBorders(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If this child is a Border, yield it
                if (child is Border border)
                {
                    yield return border;
                }

                // Recurse into children
                foreach (var descendant in FindAllBorders(child))
                {
                    yield return descendant;
                }
            }
        }


    }

}
