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
    partial class MenuPageViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [ObservableProperty]
        private CollectionViewSource menuEntrySource;

        [ObservableProperty]
        private ObservableCollection<MenuWrapper> menuList;

        [ObservableProperty]
        private string month = DateTime.Now.ToString("MMMM");
        [ObservableProperty]
        private Visibility isLoading = Visibility.Collapsed;
        [ObservableProperty]
        private string year = DateTime.Now.ToString("yyyy");
        public string Time => $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Month.ToLower())} {Year}";
        private DateTime currentdate = DateTime.Now;
        private DateTime start;
        private DateTime end;

        public MenuPageViewModel()
        {
            UpdateRange();
            WeakReferenceMessenger.Default.Register<RemoveRecipeMenuMessenger>(this, async (r, m) =>
            {
                var recipe = MenuList.FirstOrDefault(x => x.Date == m.menu.Date);
                if (recipe != null)
                {
                    if (m.IsLunch)
                    {
                        recipe.LunchRecipe = null;
                    }
                    else
                    {
                        recipe.DinnerRecipe = null;
                    }
                    MenuEntrySource.View.Refresh();
                }
            });
        }

        [RelayCommand]
        private async Task LoadMenu()
        {
            IsLoading = Visibility.Visible;

            try
            {
                var api = ApiService.Instance;
                var list = await api.GetListAsync<MenuEntry>($"api/menuentry/{currentdate}");

                MenuList = new ObservableCollection<MenuWrapper>(
                    list?.Select(x => new MenuWrapper(x)) ?? Enumerable.Empty<MenuWrapper>()
                );

                if (MenuEntrySource != null)
                {
                    MenuEntrySource.Filter -= FilterMenuEntry;
                }

                MenuEntrySource = new CollectionViewSource { Source = MenuList };
                MenuEntrySource.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
                MenuEntrySource.Filter += FilterMenuEntry;
                MenuEntrySource.View.Culture = new CultureInfo("sv-SE");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MenuList = new ObservableCollection<MenuWrapper>();
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }
            //var api = ApiService.Instance;
            //var list = await api.GetListAsync<MenuEntry>($"api/menuentry/{currentdate}");
            //if (list == null) { MenuList = new ObservableCollection<MenuWrapper>(); }
            //MenuList = new ObservableCollection<MenuWrapper>(list?.Select(x => new MenuWrapper(x)) ?? Enumerable.Empty<MenuWrapper>());
            //SetSource();
        }

        private void SetSource()
        {
            MenuEntrySource = new CollectionViewSource();
            MenuEntrySource.Source = MenuList;
            MenuEntrySource.View.Culture = new CultureInfo("sv-SE");
            MenuEntrySource.Filter += FilterMenuEntry;
            MenuEntrySource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
        }

        private void FilterMenuEntry(object sender, FilterEventArgs e)
        {
            if (e.Item is MenuWrapper wrapper)
            {
                e.Accepted = wrapper.Date >= start && wrapper.Date <= end;
            }
        }

        [RelayCommand]
        public async Task ChangeMonth(string change)
        {
            currentdate = currentdate.AddMonths((change == "true" ? 1 : -1));
            UpdateRange();
            if (start < MenuList.First().Date || end > MenuList.Last().Date)
            {
                var api = ApiService.Instance;
                var list = await api.GetListAsync<MenuEntry>($"api/menuentry/{currentdate}");
                if (list == null) { MenuList = new ObservableCollection<MenuWrapper>(); }
                MenuList = new ObservableCollection<MenuWrapper>(list?.Select(x => new MenuWrapper(x)) ?? Enumerable.Empty<MenuWrapper>());
            }
            GetTime();
            MenuEntrySource.View.Refresh();
        }

        [RelayCommand]
        public void MouseEnterLunch(int id)
        {
            var recipes = MenuList.Where(x => x.LunchRecipeId == id);
            foreach (var item in recipes)
            {
                item.IsLunchHighlighted = true;
            }
        }

        [RelayCommand]
        public void MouseLeaveLunch(int id)
        {
            var recipes = MenuList.Where(x => x.LunchRecipeId == id);
            foreach (var item in recipes)
            {
                item.IsLunchHighlighted = false;
            }
        }

        [RelayCommand]
        public void MouseEnterDinner(int id)
        {
            var recipes = MenuList.Where(x => x.DinnerRecipeId == id);
            foreach (var item in recipes)
            {
                item.IsDinnerHighlighted = true;
            }
        }

        [RelayCommand]
        public void MouseLeaveDinner(int id)
        {
            var recipes = MenuList.Where(x => x.DinnerRecipeId == id);
            foreach (var item in recipes)
            {
                item.IsDinnerHighlighted = false;
            }
        }


        private void UpdateRange()
        {
            var firstDay = new DateTime(currentdate.Year, currentdate.Month, 1).DayOfWeek;
            var numOfDays = DateTime.DaysInMonth(currentdate.Year, currentdate.Month);
            var lastDay = new DateTime(currentdate.Year, currentdate.Month, numOfDays).DayOfWeek;
            int daysBefore = (7 + (firstDay - DayOfWeek.Monday)) % 7;
            int daysAfter = (7 + (DayOfWeek.Sunday - lastDay)) % 7;
            start = new DateTime(currentdate.Year, currentdate.Month, 1).AddDays(-daysBefore);
            end = new DateTime(currentdate.Year, currentdate.Month, numOfDays).AddDays(daysAfter);
        }

        private void GetTime()
        {
            Month = currentdate.ToString("MMMM");
            Year = currentdate.ToString("yyyy");
        }

        [RelayCommand]
        public void ShowRecipesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Recipe));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Recipe));

        }

        [RelayCommand]
        public void ShowIngredientsPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Goods));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Goods));
        }


        [RelayCommand]
        public void ShowDishesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Shopping));

        }

        partial void OnMonthChanged(string value) => OnPropertyChanged(nameof(Time));
        partial void OnYearChanged(string value) => OnPropertyChanged(nameof(Time));
    }
}
