using Matsedeln.ViewModel;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
            DataContext = new MenuPageViewModel();
        }

        private void Lunch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is MenuEntry menu)
            {
                var itemscontrol = GetItemsControl(border);
                if (itemscontrol == null) return;
                var allborders = FindAllBorders(itemscontrol, "lunch");
                if (allborders == null) return;
                foreach ( var item in allborders )
                {
                    var datacontext = item.DataContext as MenuEntry;
                    if (datacontext != null)
                    {
                        if (datacontext.LunchRecipe == null) continue;
                        if (datacontext.LunchRecipeId == menu.LunchRecipeId) item.BorderBrush = Brushes.Red;
                    }
                }
            }
        }

        private void Lunch_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is MenuEntry menu)
            {
                var itemscontrol = GetItemsControl(border);
                if (itemscontrol == null) return;
                var allborders = FindAllBorders(itemscontrol, "lunch");
                if (allborders == null) return;
                foreach (var item in allborders)
                {
                    if (item.BorderBrush == Brushes.Red) item.BorderBrush = Brushes.Transparent;
                }
            }
        }
        private void Dinner_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is MenuEntry menu)
            {
                var itemscontrol = GetItemsControl(border);
                if (itemscontrol == null) return;
                var allborders = FindAllBorders(itemscontrol, "dinner");
                if (allborders == null) return;
                foreach (var item in allborders)
                {
                    var datacontext = item.DataContext as MenuEntry;
                    if (datacontext != null)
                    {
                        if (datacontext.DinnerRecipe == null) continue;
                        if (datacontext.DinnerRecipeId == menu.DinnerRecipeId) item.BorderBrush = Brushes.Red;
                    }
                }
            }
        }

        private void Dinner_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is MenuEntry menu)
            {
                var itemscontrol = GetItemsControl(border);
                if (itemscontrol == null) return;
                var allborders = FindAllBorders(itemscontrol, "dinner");
                if (allborders == null) return;
                foreach (var item in allborders)
                {
                    if (item.BorderBrush == Brushes.Red) item.BorderBrush = Brushes.Transparent;
                }
            }
        }

        public IEnumerable<Border> FindAllBorders(DependencyObject parent, string meal)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If this child is a Border, yield it
                if (child is Border border && border.Tag != null && border.Tag.ToString() == meal)
                {
                    yield return border;
                }

                // Recurse into children
                foreach (var descendant in FindAllBorders(child, meal))
                {
                    yield return descendant;
                }
            }
        }

        private ItemsControl? GetItemsControl(Border b)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(b);

            ItemsControl? control = null;

            while (parent != null)
            {
                if (parent is ItemsControl ic)
                {
                    control = ic;
                    return control;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return control;
        }

    }
}
