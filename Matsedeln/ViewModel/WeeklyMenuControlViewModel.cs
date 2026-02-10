using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    public partial class WeeklyMenuControlViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        private int weeknumber;
        [ObservableProperty]
        private string weekAndDate = "";
        private bool _lunchOrDinner;
        private MenuWrapper currentEntry;
        [ObservableProperty]
        private ObservableCollection<MenuWrapper> menuList;
        [ObservableProperty]
        private CollectionViewSource menuEntrySource;
        [ObservableProperty]
        private Visibility isLoading = Visibility.Collapsed;
        private CultureInfo culture = new CultureInfo("sv-SE");
        private System.Globalization.Calendar calendar;
        private DateTime WeekToDisplay = DateTime.Today;

        public WeeklyMenuControlViewModel()
        {
            calendar = culture.Calendar;
            DisplayWeek();
            WeakReferenceMessenger.Default.Register<AddUpdateMenu>(this, (r, m) => UpdateMenu(m.wrapper));

        }

        [RelayCommand]
        private async Task LoadWeeklyMenu()
        {
            IsLoading = Visibility.Visible;

            try
            {
                var api = ApiService.Instance;
                var list = await api.GetListAsync<MenuEntry>($"api/menuentry/{WeekToDisplay}");

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
                MenuList = new ObservableCollection<MenuWrapper>();
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }

        }


        private void FilterMenuEntry(object sender, FilterEventArgs e)
        {
            if (e.Item is MenuWrapper wrapper)
            {
                e.Accepted = wrapper.Date >= WeekToDisplay && wrapper.Date < WeekToDisplay.AddDays(7);
            }
        }

        private async void UpdateMenu(RecipeWrapper wrapper)
        {
            if (_lunchOrDinner) currentEntry.LunchRecipe = wrapper.Recipe;
            else currentEntry.DinnerRecipe = wrapper.Recipe;
            var api = ApiService.Instance;
            await api.PostAsync("api/menuentry", currentEntry.Menu);
            wrapper.IsHighlighted = false;
            MenuEntrySource.View.Refresh();
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Menu));
        }

        [RelayCommand]
        public async Task RemoveLunch(MenuWrapper wrapper)
        {
            if (wrapper == null) return;
            wrapper.LunchRecipe = null;
            wrapper.LunchRecipeId = null;
            var api = ApiService.Instance;
            var result = await api.PostAsync($"api/menuentry", wrapper.Menu);
            if (!result)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Kunde inte ta bort maträtt från matsedel.", isError: true));
                return;
            }
            wrapper.LunchRecipe = null;
            WeakReferenceMessenger.Default.Send(new RemoveRecipeMenuMessenger(wrapper, true));
        }
        [RelayCommand]
        public async Task RemoveDinner(MenuWrapper wrapper)
        {
            if (wrapper == null) return;
            wrapper.DinnerRecipe = null;
            wrapper.DinnerRecipeId = null;
            var api = ApiService.Instance;
            var result = await api.PostAsync($"api/menuentry", wrapper.Menu);
            if (!result)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Kunde inte ta bort maträtt från matsedel.", isError: true));
                return;
            }
            wrapper.DinnerRecipe = null;
            WeakReferenceMessenger.Default.Send(new RemoveRecipeMenuMessenger(wrapper, false));
        }

        private async void DisplayWeek()
        {
            int diff = (7 + (WeekToDisplay.DayOfWeek - DayOfWeek.Monday)) % 7;
            weeknumber = calendar.GetWeekOfYear(WeekToDisplay, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
            WeekToDisplay = WeekToDisplay.AddDays(-diff);
            WeekAndDate = $"{WeekToDisplay.Year} - Vecka {weeknumber}: {WeekToDisplay.Date.ToString("d/M", CultureInfo.InvariantCulture)} - {WeekToDisplay.AddDays(6).Date.ToString("d/M", CultureInfo.InvariantCulture)}";
        }

        [RelayCommand]
        private void PrevWeek()
        {
            WeekToDisplay = WeekToDisplay.AddDays(-7);
            DisplayWeek();
            MenuEntrySource.View.Refresh();

        }
        [RelayCommand]
        private void NextWeek()
        {
            WeekToDisplay = WeekToDisplay.AddDays(7);
            DisplayWeek();
            MenuEntrySource.View.Refresh();
        }
        [RelayCommand]
        private void LunchChanged(MenuWrapper wrapper)
        {
            currentEntry = wrapper;
            _lunchOrDinner = true;
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
        }
        [RelayCommand]
        private void DinnerChanged(MenuWrapper wrapper)
        {
            currentEntry = wrapper;
            _lunchOrDinner = false;
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
        }


    }


}
