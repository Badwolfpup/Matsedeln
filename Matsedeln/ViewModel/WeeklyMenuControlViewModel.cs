using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matsedeln.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    public partial class WeeklyMenuControlViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;

        private int weeknumber;
        [ObservableProperty]
        private string weekAndDate = "";



        
        public CollectionViewSource MenuListSource { get; set; }

        private CultureInfo culture = new CultureInfo("sv-SE");
        private System.Globalization.Calendar calendar;
        private DateTime WeekToDisplay = DateTime.Today;

        public WeeklyMenuControlViewModel()
        {
            calendar = culture.Calendar;
            DisplayWeek();
            GetMenuListFromDB();
        }

        private async void GetMenuListFromDB()
        {
            //Ad.MenuList = await DBHelper.GetAllMenuItemsFromDB();
            MenuListSource = new CollectionViewSource();
            MenuListSource.Source = Ad.MenuList;
        }

        private void DisplayWeek()
        {
            int diff = (7 + (WeekToDisplay.DayOfWeek - DayOfWeek.Monday)) % 7;
            weeknumber = calendar.GetWeekOfYear(WeekToDisplay, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
            WeekAndDate = $"{WeekToDisplay.Year} - Vecka {weeknumber}: {WeekToDisplay.AddDays(-diff).Date.ToString("d/M", CultureInfo.InvariantCulture)} - {WeekToDisplay.AddDays(-diff + 6).Date.ToString("d/M", CultureInfo.InvariantCulture)}";
        }
        [RelayCommand]
        private void PrevWeek()
        {
            WeekToDisplay = WeekToDisplay.AddDays(-7);
            DisplayWeek();

        }
        [RelayCommand]
        private void NextWeek()
        {
            WeekToDisplay = WeekToDisplay.AddDays(7);
            DisplayWeek();
        }
        [RelayCommand]
        private void LunchSelectionChanged()
        {

        }
        [RelayCommand]
        private void DinnerSelectionChanged()
        {

        }
    }
}
