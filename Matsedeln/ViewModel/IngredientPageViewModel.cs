using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Usercontrols;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static Matsedeln.ViewModel.IngredientPageViewModel;

namespace Matsedeln.ViewModel
{
    public partial class IngredientPageViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;

        public IngredientPageViewModel()
        {
            GoodsViewSource = new CollectionViewSource();
            GoodsViewSource.Source = Ad.GoodsList;
            GoodsViewSource.Filter += FilterGoods;
            WeakReferenceMessenger.Default.Register<AppData.RefreshCollectionViewMessage>(this, (r, m) => GoodsViewSource.View.Refresh());
        }

        public CollectionViewSource GoodsViewSource { get; set; }

        private void FilterGoods(object sender, FilterEventArgs e)
        {
            if (e.Item is Goods good)
            {
                if (!string.IsNullOrEmpty(Ad.FilterText))
                {
                    if (good.Name.IndexOf(Ad.FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = true;
                }
            }

        }

        [RelayCommand]
        private void SelectBorder(Border border)
        {
            WeakReferenceMessenger.Default.Send(new AppData.SelectedBorderMessage(border));

        }

    }
}
