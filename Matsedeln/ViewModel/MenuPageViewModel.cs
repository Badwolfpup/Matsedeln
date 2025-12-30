using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    partial class MenuPageViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;


        public CollectionViewSource MenuEntrySource { get; set; }

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
            CreateMonth();
            UpdateRange();
            MenuEntrySource = new CollectionViewSource();
            MenuEntrySource.Source = Ad.MenuList;
            MenuEntrySource.Filter += FilterMenuEntry;
            MenuEntrySource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            MenuEntrySource.View.Refresh();
            WeakReferenceMessenger.Default.Register<AppData.RefreshMenuEntrySourceMessage>(this, (r, m) => MenuEntrySource.View.Refresh());

        }


        private void FilterMenuEntry(object sender, FilterEventArgs e)
        {
            if (e.Item is MenuEntry menuEntry)
            {
                e.Accepted = menuEntry.Date >= start && menuEntry.Date <= end;
            }
        }

        private async void CreateMonth()
        {
            await Ad.MenuService.AddEmptyDays(currentdate);
            GetTime();
        }

        [RelayCommand]
        public async Task PrevMonth()
        {
            currentdate = currentdate.AddMonths(-1);
            UpdateRange();
            await Ad.MenuService.AddEmptyDays(currentdate);
            GetTime();
            MenuEntrySource.View.Refresh();
        }
        [RelayCommand]
        public async Task NextMonth()
        {
            currentdate = currentdate.AddMonths(1);
            UpdateRange();
            await Ad.MenuService.AddEmptyDays(currentdate);
            GetTime();
            MenuEntrySource.View.Refresh();
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

        partial void OnMonthChanged(string value) => OnPropertyChanged(nameof(Time));
        partial void OnYearChanged(string value) => OnPropertyChanged(nameof(Time));
    }
}
