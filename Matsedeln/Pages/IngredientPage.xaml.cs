using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Usercontrols;
using Matsedeln.Usercontrols;
using Matsedeln.ViewModel;
using MatsedelnShared.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using static Matsedeln.ViewModel.IngredientPageViewModel;

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


        private Border _selectedBorder;
        public AppData Ad = AppData.Instance;

        




        public IngredientPage()
        {
            InitializeComponent();
            DataContext = new IngredientPageViewModel();
            WeakReferenceMessenger.Default.Register<AppData.SelectedBorderMessage>(this, (r, m) => SelectBorder(m.Border, m.GoodsOrRecipe));
            WeakReferenceMessenger.Default.Register<AppData.MoveBorderMessage>(this, (r, m) => MoveBorder(m.Goods));
            WeakReferenceMessenger.Default.Register<AppData.ResetBorderMessage>(this, (r, m) => ResetBorderSelection());
            WeakReferenceMessenger.Default.Register<AppData.RemoveHighlightBorderMessage>(this, (r, m) => RemoveHighlightBorder(m.Goods));
            WeakReferenceMessenger.Default.Register<AppData.RemoveAllHighlightBorderMessage>(this, (r, m) => RemoveAllHighlightBorder(Ad.GoodsList));
        }

        private void SelectBorder(Border border, bool isGoodsUC)
        {
            
            if (border.DataContext is Goods good && good != null)
            {
                if (isGoodsUC)
                {
                    if (_selectedBorder == null)
                    {
                        _selectedBorder = border;
                        _selectedBorder.BorderBrush = Brushes.Red;
                        WeakReferenceMessenger.Default.Send(new AppData.PassGoodsToUCMessage(good));

                    }
                    else
                    {
                        if (border == _selectedBorder)
                        {
                            _selectedBorder.BorderBrush = Brushes.Transparent;
                            _selectedBorder = null;
                            WeakReferenceMessenger.Default.Send(new AppData.PassGoodsToUCMessage(new Goods()));

                        }
                        else
                        {
                            _selectedBorder.BorderBrush = Brushes.Transparent;
                            _selectedBorder = border;
                            _selectedBorder.BorderBrush = Brushes.Red;
                            WeakReferenceMessenger.Default.Send(new AppData.PassGoodsToUCMessage(good));

                        }
                    }
                } 
                else
                {
                    if (border.BorderBrush != Brushes.Red)
                    {
                        Goods oldgood = null;

                        if (_selectedBorder != null) oldgood = _selectedBorder.DataContext as Goods;
                        if (oldgood != null) WeakReferenceMessenger.Default.Send(new AppData.IsGoodAddedToIngredientMessage(oldgood));
                        _selectedBorder = border;
                        border.BorderBrush = Brushes.Red;
                        WeakReferenceMessenger.Default.Send(new AppData.PassGoodsToUCMessage(good));
                    }
                }


            }
        }

        private void RemoveHighlightBorder(Goods goods)
        {
            if (goods == null) return;


            // Get the visual container for this item
            var container = GoodsItemsControl.ItemContainerGenerator.ContainerFromItem(goods) as FrameworkElement;
            if (container == null) return;  // Item not generated yet (e.g., virtualized)

            // Traverse the visual tree to find the Border (adjust based on your template structure)
            var border = FindVisualChild<Border>(container);
            if (border != null)
            {
                border.BorderBrush = Brushes.Transparent;
            }
        }

        private void RemoveAllHighlightBorder(ObservableCollection<Goods> goods)
        {
            if (goods == null) return;

            foreach (var item in goods)
            {
                // Get the visual container for this item
                var container = GoodsItemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container == null) return;  // Item not generated yet (e.g., virtualized)

                // Traverse the visual tree to find the Border (adjust based on your template structure)
                var border = FindVisualChild<Border>(container);
                if (border != null)
                {
                    border.BorderBrush = Brushes.Transparent;
                }
            }
        }

        private void MoveBorder(Goods goods)
        {
            if (goods == null) return;


            // Get the visual container for this item
            var container = GoodsItemsControl.ItemContainerGenerator.ContainerFromItem(goods) as FrameworkElement;
            if (container == null) return;  // Item not generated yet (e.g., virtualized)

            // Traverse the visual tree to find the Border (adjust based on your template structure)
            var border = FindVisualChild<Border>(container);
            if (border != null)
            {
                _selectedBorder.BorderBrush = Brushes.Transparent;
                _selectedBorder = border;
                _selectedBorder.BorderBrush = Brushes.Red;
            }
        }

        private void ResetBorderSelection()
        {
            if (_selectedBorder != null)
            {
                _selectedBorder.BorderBrush = Brushes.Transparent;
                _selectedBorder = null;
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
