using Matsedeln;
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
using Matsedeln.Usercontrols;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for IngredientPage.xaml
    /// </summary>
    public partial class IngredientPage : Page, INotifyPropertyChanged
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

        public IngredientPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            DataContext = main;
        }

        private void Select_Border(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Goods good)
            {
                if (SelectedBorder == null)
                {
                    SelectedBorder = border;
                    SelectedBorder.BorderBrush = Brushes.Red;
                }
                else
                {
                    if (border == SelectedBorder)
                    {
                        SelectedBorder.BorderBrush = Brushes.Transparent;
                        SelectedBorder = null;
                        mainWindow.SelectedGood = null;
                        return;
                    }
                    else
                    {
                        SelectedBorder.BorderBrush = Brushes.Transparent;
                        SelectedBorder = border;
                        SelectedBorder.BorderBrush = Brushes.Red;
                    }
                }
                if (!mainWindow.EnableAddGoodsBtn) mainWindow.AddContentControl.Content = new NewGoodsControl(mainWindow, good);
                else mainWindow.SelectedGood = good;

            }
        }

        public void MoveBorder(Goods goods)
        {
            if (goods == null) return;


            // Get the visual container for this item
            var container = GoodsItemsControl.ItemContainerGenerator.ContainerFromItem(goods) as FrameworkElement;
            if (container == null) return;  // Item not generated yet (e.g., virtualized)

            // Traverse the visual tree to find the Border (adjust based on your template structure)
            var border = FindVisualChild<Border>(container);
            if (border != null)
            {
                SelectedBorder.BorderBrush = Brushes.Transparent;
                SelectedBorder = border;
                SelectedBorder.BorderBrush = Brushes.Red;
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}
