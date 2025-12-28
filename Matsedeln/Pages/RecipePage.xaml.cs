using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Usercontrols;
using Matsedeln.ViewModel;
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


        public AppData Ad { get; }  = AppData.Instance;

        public RecipePage()
        {
            InitializeComponent();
            DataContext = new RecipePageViewModel();
            Loaded += RecipePage_Loaded; ;
        }

        private void RecipePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is RecipePageViewModel vm)
            {
                vm.OnPageLoaded();  // Call the method directly
            }
        }

        private void Select_Border(object sender, MouseButtonEventArgs e)
        {
            if (Ad.CurrentUserControl is ShoppingListControl shop && shop.ShowShoppinglist) return;

            if (sender is Border border && border.DataContext is Recipe recipe)
            {
                if (border == null) return;
                WeakReferenceMessenger.Default.Send(new AppData.AddRecipeToMenuMessage(recipe));

                if (border.BorderBrush == Brushes.Red)
                {
                    border.BorderBrush = Brushes.Transparent;
                    WeakReferenceMessenger.Default.Send(new AppData.RemoveIngredientShoplistMessage(recipe));
                    return;
                }
                else
                {
                    border.BorderBrush = Brushes.Red;
                    WeakReferenceMessenger.Default.Send(new AppData.AddIngredientShopListMessage(recipe));
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
