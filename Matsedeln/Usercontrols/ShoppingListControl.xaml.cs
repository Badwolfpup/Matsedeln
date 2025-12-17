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

        private readonly MainWindow main;

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
        }

        public ShoppingListControl(MainWindow mainWindow)
        {
            InitializeComponent();
            main = mainWindow;
            DataContext = mainWindow;
            mainWindow.ShoppingList = new();
        }

        private void MakeShoppinglist_Click(object sender, RoutedEventArgs e)
        {
            ShowIngredients = false;
            ShowShoppinglist = true;
        }

        private void AbortIngredientlist_Click(object sender, RoutedEventArgs e)
        {
            main.ShoppingList.Clear();
            var allBorders = FindAllBorders(main.RecipePageInstance.RecipeItemsControl);

            foreach (var border in allBorders)
            {
                border.BorderBrush = Brushes.Transparent;
            }
        }

        private void CopyShoppinglist_Click(object sender, RoutedEventArgs e)
        {
            string result = "";

            foreach (var item in main.ShoppingList)
            {
                result += item.ToString() +"\n";
            }
            //CopyList();
            Clipboard.SetText(string.Join(Environment.NewLine, main.ShoppingList.Select(x => x.ToString())));
        }

        private void CopyList()
        {
            int qmax = main.ShoppingList.Max(x => x.Quantity).ToString().Length;
            int umax = main.ShoppingList.Max(y => y.Unit.Length);
            if (qmax == 0 || umax == 0) return;
            Clipboard.SetText(string.Join(Environment.NewLine, main.ShoppingList.Select(x =>
            {
                int qlength = x.Quantity.ToString().Length;
                int ulength = x.Unit.Length;
                string quantity = $"{(qlength < qmax ? string.Concat(Enumerable.Repeat(" ", qmax - qlength)) : "")}{x.Quantity}";
                string unit = $"{x.Unit}{(ulength < umax ? string.Concat(Enumerable.Repeat(" ", umax - ulength)) : "")}";
                return $"{quantity} {unit} {x.Good.Name}";
            })));

        }

        private void AbortShoppinglist_Click(object sender, RoutedEventArgs e)
        {
            main.ShoppingList.Clear();
            var allBorders = FindAllBorders(main.RecipePageInstance.RecipeItemsControl);

            foreach (var border in allBorders)
            {
                border.BorderBrush = Brushes.Transparent;
            }
            ShowShoppinglist = false;
            ShowIngredients = true;
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

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var ingredient = comboBox.DataContext as Ingredient;
            if (ingredient == null) return;

            if (comboBox.SelectedItem == "g") { ingredient.Unit = "g"; ingredient.Quantity = ingredient.QuantityInGram; }
            else if (comboBox.SelectedItem == "dl") { ingredient.Unit = "dl"; ingredient.Quantity = ingredient.QuantityInDl; }
            else if (comboBox.SelectedItem == "st") { ingredient.Unit = "st"; ingredient.Quantity = ingredient.QuantityInSt; }



        }

    }
}
