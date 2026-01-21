using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.ViewModel;
using System;
using System.Collections.Generic;
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

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for ShoppingListControl.xaml
    /// </summary>
    public partial class ShoppingListControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public AppData Ad { get; } = AppData.Instance;

        private bool showingredients = true;
        private bool showshoppinglist;

        public bool ShowIngredients
        {
            get => showingredients;
            set
            {
                if (showingredients != value)
                {
                    showingredients = value;
                    OnPropertyChanged(nameof(ShowIngredients));
                }
            }
        }

        public bool ShowShoppinglist
        {
            get => showshoppinglist;
            set
            {
                if (showshoppinglist != value)
                {
                    showshoppinglist = value;
                    OnPropertyChanged(nameof(ShowShoppinglist));
                }
            }
        }

        public ShoppingListControl()
        {
            InitializeComponent();
            DataContext = new ShoppingListViewModel();
            Ad.ShoppingList = new();
            WeakReferenceMessenger.Default.Register<FindBorderShopListMessage>(this, (r, m) =>
            {
                var foundBorder = FindAllBorders(m.ItemsControl); 
                m.Reply(foundBorder);
            });
            WeakReferenceMessenger.Default.Register<AppData.ShowShoppingListMessage>(this, (r, m) => {
                ShowShoppinglist = false;
                ShowIngredients = true;
            });


        }

        private void MakeShoppinglist_Click(object sender, RoutedEventArgs e)
        {
            ShowIngredients = false;
            ShowShoppinglist = true;
        }



        private IEnumerable<Border> FindAllBorders(DependencyObject parent)
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
