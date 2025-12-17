using Matsedeln.Usercontrols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for RecipePage.xaml
    /// </summary>
    public partial class RecipePage : Page, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly MainWindow mainWindow;

        private Border selectedBorder;
        public Border SelectedBorder
        {
            get { return selectedBorder; }
            set
            {
                if (selectedBorder != value)
                {
                    selectedBorder = value;
                    if (mainWindow.EnableAddGoodsBtn && selectedBorder != null && mainWindow.AddContentControl.Content is NewRecipeControl recipeControl)
                    {
                        recipeControl.ResetInput();
                    }
                    OnPropertyChanged(nameof(SelectedBorder));
                }
            }
        }

        public ObservableCollection<Recipe> RecipeList {get; set; }
        public RecipePage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            DataContext = main;
            
        }

        private void Select_Border(object sender, MouseButtonEventArgs e)
        {
            if (mainWindow.AddContentControl.Content is ShoppingListControl shop && shop.ShowShoppinglist) return;

            if (sender is Border border && border.DataContext is Recipe recipe)
            {
                if (border == null) return;

                if (border.BorderBrush == Brushes.Red)
                {
                    border.BorderBrush = Brushes.Transparent;
                    RemoveIngredientsFromShoppinglist(recipe);
                    return;
                }
                else
                {
                    border.BorderBrush = Brushes.Red;
                    AddIngredientToShoppinglist(recipe);
                    return;
                }


            }
        }

        private void AddIngredientToShoppinglist(Recipe recipe)
        {
            if (mainWindow.AddContentControl.Content is ShoppingListControl shop && shop.ShowShoppinglist) return;
            recipe.Ingredientlist.ToList().ForEach(ingredient =>
            {
                if (!mainWindow.ShoppingList.Any(i => i.Good.Name == ingredient.Good.Name))
                {
                    mainWindow.ShoppingList.Add(new Ingredient(ingredient));
                }
                else
                {
                    var existingItem = mainWindow.ShoppingList.First(i => i.Good.Name == ingredient.Good.Name);
                    existingItem.QuantityInGram += ingredient.QuantityInGram;
                    existingItem.QuantityInDl += ingredient.QuantityInDl;
                    existingItem.QuantityInSt += ingredient.QuantityInSt;
                    existingItem.QuantityInMsk += ingredient.QuantityInMsk;
                    existingItem.QuantityInTsk += ingredient.QuantityInTsk;
                }
            });
        }


        private void RemoveIngredientsFromShoppinglist(Recipe recipe)
        {
            if (mainWindow.AddContentControl.Content is ShoppingListControl shop && shop.ShowShoppinglist) return;

            recipe.Ingredientlist.ToList().ForEach(ingredient =>
            {
                var itemToRemove = mainWindow.ShoppingList.FirstOrDefault(i => i.Good.Name == ingredient.Good.Name);
                if (itemToRemove != null)
                {
                    if (itemToRemove.QuantityInGram == ingredient.QuantityInGram) mainWindow.ShoppingList.Remove(itemToRemove);
                    else
                    {
                        itemToRemove.QuantityInGram -= ingredient.QuantityInGram;
                        itemToRemove.QuantityInDl -= ingredient.QuantityInDl;
                        itemToRemove.QuantityInSt -= ingredient.QuantityInSt;
                        itemToRemove.QuantityInMsk -= ingredient.QuantityInMsk;
                        itemToRemove.QuantityInTsk -= ingredient.QuantityInTsk;
                    }
                }
            });
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
