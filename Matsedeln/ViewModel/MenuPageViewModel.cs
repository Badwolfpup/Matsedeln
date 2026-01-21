using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    partial class MenuPageViewModel: ObservableObject
    {

        [ObservableProperty]
        private CollectionViewSource menuEntrySource;

        [ObservableProperty]
        private ObservableCollection<MenuEntry> menuList;

        [ObservableProperty]
        private string month = DateTime.Now.ToString("MMMM");

        [ObservableProperty]
        private string year = DateTime.Now.ToString("yyyy");
        public string Time => $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Month.ToLower())} {Year}";
        private DateTime currentdate = DateTime.Now;
        private DateTime start;
        private DateTime end;

        public MenuPageViewModel()
        {
            UpdateRange();
            GetMenuFromDb();
            WeakReferenceMessenger.Default.Register<AppData.RefreshMenuEntrySourceMessage>(this, (r, m) => MenuEntrySource.View.Refresh());

        }


        private async void GetMenuFromDb()
        {
            var api = new ApiService();
            var list = await api.GetListAsync<MenuEntry>($"api/menuentry/{currentdate}");
            if (list == null) { MenuList = new ObservableCollection<MenuEntry>(); }
            else MenuList = new ObservableCollection<MenuEntry>(list);
            SetSource();
        }

        private void SetSource()
        {
            MenuEntrySource = new CollectionViewSource();
            MenuEntrySource.Source = MenuList;
            MenuEntrySource.Filter += FilterMenuEntry;
            MenuEntrySource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            MenuEntrySource.View.Refresh();
        }

        private void FilterMenuEntry(object sender, FilterEventArgs e)
        {
            if (e.Item is MenuEntry menuEntry)
            {
                e.Accepted = menuEntry.Date >= start && menuEntry.Date <= end;
            }
        }

        [RelayCommand]
        public async Task ChangeMonth(string change)
        {
            currentdate = currentdate.AddMonths((change == "true" ? 1 : -1));
            UpdateRange();
            var api = new ApiService();
            var list = await api.GetListAsync<MenuEntry>($"api/menu/{currentdate}");
            if (list == null) { MenuList = new ObservableCollection<MenuEntry>(); }
            else MenuList = new ObservableCollection<MenuEntry>(list);
            GetTime();
            MenuEntrySource.View.Refresh();
        }
        //[RelayCommand]
        //public async Task NextMonth()
        //{
        //    currentdate = currentdate.AddMonths(1);
        //    UpdateRange();
        //    var api = new ApiService();
        //    var list = await api.GetListAsync<MenuEntry>($"api/menu/{currentdate}");
        //    if (list == null) { MenuList = new ObservableCollection<MenuEntry>(); }
        //    else MenuList = new ObservableCollection<MenuEntry>(list);
        //    GetTime();
        //    MenuEntrySource.View.Refresh();
        //}

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
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("recipe"));
        }

        [RelayCommand]
        public void ShowIngredientsPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger("goods"));
        }

        partial void OnMonthChanged(string value) => OnPropertyChanged(nameof(Time));
        partial void OnYearChanged(string value) => OnPropertyChanged(nameof(Time));
    }
}
