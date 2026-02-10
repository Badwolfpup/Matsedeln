using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    public partial class IngredientPageViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
        [ObservableProperty]
        private GoodsWrapper selectedGood;
        [ObservableProperty]
        private string filterText = string.Empty;
        [ObservableProperty]
        private ObservableCollection<GoodsWrapper> goodsList;
        [ObservableProperty]
        private Visibility isLoading = Visibility.Collapsed;
        [ObservableProperty]
        private CollectionViewSource goodsViewSource;

        public IngredientPageViewModel()
        {
            WeakReferenceMessenger.Default.Register<NameExistsMessenger>(this, async (r, m) => { if (GoodsList != null && GoodsList.Count > 0) m.Reply(GoodsList.Any(x => x.Name == m.Name)); });
            WeakReferenceMessenger.Default.Register<RefreshListMessenger>(this, async (r, m) => await LoadGoods());
            WeakReferenceMessenger.Default.Register<SetSelectedMessenger>(this, (r, m) =>
            {
                var goodWrapper = GoodsList?.FirstOrDefault(x => x.Id == m.Id);
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
                    var goodWrapper = GoodsList.FirstOrDefault(x => x.Id == m.Id);
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
            IsLoading = Visibility.Visible;

            try
            {
                var api = ApiService.Instance;
                var list = await api.GetListAsync<Goods>("api/goods");

                GoodsList = new ObservableCollection<GoodsWrapper>(
                    list?.Select(x => new GoodsWrapper(x)) ?? Enumerable.Empty<GoodsWrapper>()
                );

                if (GoodsViewSource != null)
                {
                    GoodsViewSource.Filter -= FilterGoods;
                }

                GoodsViewSource = new CollectionViewSource { Source = GoodsList };
                GoodsViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                GoodsViewSource.Filter += FilterGoods;
                GoodsViewSource.View.Culture = new CultureInfo("sv-SE");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                GoodsList = new ObservableCollection<GoodsWrapper>();
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }
        }


        [RelayCommand]
        public async Task DeleteGood()
        {
            if (SelectedGood == null)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Du måste välja något innan du kan radera.", isError: true));
                return;
            }
            MessageBoxResult result = MessageBox.Show("Vill du verkligen radera denna vara?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var api = ApiService.Instance;
                if (!await api.DeleteAsync($"api/goods/{SelectedGood.Good.Id}"))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Det gick inte att radera varan", isError: true));
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
                    e.Accepted = wrapper.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 || wrapper.IsSelected;
                }
            }

        }

        [RelayCommand]
        private void SelectBorder(GoodsWrapper wrapper)
        {
            if (wrapper == null) return;
            //GoodsList.ToList().ForEach(x => x.IsHighlighted = false);
            wrapper.IsHighlighted = !wrapper.IsHighlighted;
            SelectedGood = wrapper.IsHighlighted ? wrapper : null;
            WeakReferenceMessenger.Default.Send(new SelectedGoodsMessenger(SelectedGood));
        }

        partial void OnFilterTextChanged(string value)
        {
            GoodsViewSource.View.Refresh();
        }

        [RelayCommand]
        public void ShowRecipesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Recipe));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Recipe));
        }

        [RelayCommand]
        public void ShowMenuPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Menu));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Menu));

        }

        [RelayCommand]
        public void ShowDishesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Shopping));

        }
    }
}
