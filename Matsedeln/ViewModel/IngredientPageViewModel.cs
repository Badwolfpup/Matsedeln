using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        [ObservableProperty]
        private GoodsWrapper selectedGood;
        [ObservableProperty]
        private string filterText = string.Empty;
        [ObservableProperty]
        private ObservableCollection<GoodsWrapper> goodsList;

        [ObservableProperty]
        private CollectionViewSource goodsViewSource;
        public IngredientPageViewModel()
        {
            WeakReferenceMessenger.Default.Register<NameExistsMessenger>(this, async (r, m) => { if (GoodsList != null && GoodsList.Count > 0) m.Reply(GoodsList.Any(x => x.good.Name == m.Name)); });
            WeakReferenceMessenger.Default.Register<RefreshListMessenger>(this, async (r, m) => await LoadGoods());
            WeakReferenceMessenger.Default.Register<SetSelectedMessenger>(this, (r, m) =>
            {
                var goodWrapper = GoodsList.FirstOrDefault(x => x.good.Id == m.Id);
                if (goodWrapper != null)
                {
                    goodWrapper.IsSelected = true;
                }
            });
            WeakReferenceMessenger.Default.Register<ClearSelectedMessenger>(this, (r, m) =>
            {
                if (m.Id == 0)
                {
                    foreach (var good in GoodsList)
                    {
                        good.IsSelected = false;
                        good.IsHighlighted = false;
                    }
                }
                else
                {
                    var goodWrapper = GoodsList.FirstOrDefault(x => x.good.Id == m.Id);
                    if (goodWrapper != null)
                    {
                        goodWrapper.IsSelected = false;
                        goodWrapper.IsHighlighted = false;
                    }

                }
            });
        }

 
        [RelayCommand]
        private async Task LoadGoods()
        {
            var api = new ApiService();
            var list = await api.GetListAsync<Goods>("api/goods");
            if (list == null) { GoodsList = new ObservableCollection<GoodsWrapper>(); }
            GoodsList = new ObservableCollection<GoodsWrapper>(list?.Select(x => new GoodsWrapper { good = x }) ?? Enumerable.Empty<GoodsWrapper>());
            SetSource();

        }


        private void SetSource()
        {
            GoodsViewSource = new CollectionViewSource();
            GoodsViewSource.Source = GoodsList;
            GoodsViewSource.View.Culture = new CultureInfo("sv-SE");
            GoodsViewSource.SortDescriptions.Add(new SortDescription("good.Name", ListSortDirection.Ascending));
            GoodsViewSource.Filter += FilterGoods;
        }

        [RelayCommand]
        public async Task DeleteGood()
        {
            if (SelectedGood == null)
            {
                MessageBox.Show("Du måste välja något innan du kan radera.");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Vill du verkligen radera denna vara?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var api = new ApiService();
                if (!await api.DeleteAsync($"api/goods/{SelectedGood.good.Id}"))
                {
                    MessageBox.Show("Det gick inte att radera varan");
                    return;
                }
                GoodsList.Remove(SelectedGood);
                GoodsViewSource.View.Refresh();
            }
        }

        private void FilterGoods(object sender, FilterEventArgs e)
        {
            if (e.Item is GoodsWrapper wrapper)
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = wrapper.good.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 || wrapper.IsSelected;
                }
            }

        }

        [RelayCommand]
        private void SelectBorder(GoodsWrapper wrapper)
        {
            if (wrapper == null) return; 
            GoodsList.ToList().ForEach(x => x.IsHighlighted = false);
            wrapper.IsHighlighted = !wrapper.IsHighlighted;
            SelectedGood = wrapper;
            WeakReferenceMessenger.Default.Send(new SelectedGoodsMessenger(wrapper));
        }

        partial void OnFilterTextChanged(string value)
        {
            GoodsViewSource.View.Refresh();
        }

        [RelayCommand]
        public void ShowRecipesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("recipe"));
        }

        [RelayCommand]
        public void ShowMenuPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("menu"));
        }
    }
}
