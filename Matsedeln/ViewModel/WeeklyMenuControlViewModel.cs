using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace Matsedeln.ViewModel
{
    public partial class WeeklyMenuControlViewModel: ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;

        private int weeknumber;
        [ObservableProperty]
        private string weekAndDate = "";

        private bool lunchordinner;
        private MenuEntry currentEntry;
        [ObservableProperty]
        public ObservableCollection<Recipe> localList;
        public CollectionViewSource MenuListSource { get; set; }
        private CultureInfo culture = new CultureInfo("sv-SE");
        private System.Globalization.Calendar calendar;
        private DateTime WeekToDisplay = DateTime.Today;

        public WeeklyMenuControlViewModel()
        {
            calendar = culture.Calendar;
            DisplayWeek();
            MenuListSource = new CollectionViewSource();
            MenuListSource.Source = Ad.MenuList;
            MenuListSource.Filter += FilterMenuEntry;
            MenuListSource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));

            WeakReferenceMessenger.Default.Register<AppData.AddRecipeToMenuMessage>(this, (r, m) => UpdateMenu(m.recipe));
            WeakReferenceMessenger.Default.Register<AppData.RefreshMenuEntrySourceMessage>(this, (r, m) => MenuListSource.View.Refresh());
        }

        private void FilterMenuEntry(object sender, FilterEventArgs e)
        {
            if (e.Item is MenuEntry menuEntry)
            {
                e.Accepted = menuEntry.Date >= WeekToDisplay && menuEntry.Date < WeekToDisplay.AddDays(7);
            }
        }

        private async void UpdateMenu(Recipe recipe)
        {
            if (Ad.CurrentUserControl is not WeeklyMenuControl) return;
            if (lunchordinner)
            {
                currentEntry.LunchRecipe = recipe;
                if (!await Ad.MenuService.UpdateLunchEntry(currentEntry)) MessageBox.Show("Kunde inte uppdatera maträtt för lunch.");
                Ad.CurrentPage = Ad.MenuPageInstance;
            }
            else
            {
                currentEntry.DinnerRecipe = recipe;
                if (!await Ad.MenuService.UpdateDinnerEntry(currentEntry)) MessageBox.Show("Kunde inte uppdatera maträtt för middag.");
                Ad.CurrentPage = Ad.MenuPageInstance;
            }
            WeakReferenceMessenger.Default.Send(new AppData.RefreshMenuEntrySourceMessage());
        }

        [RelayCommand]
        public async Task RemoveLunch(MenuEntry menuEntry)
        {
            if (menuEntry == null) return;
            menuEntry.LunchRecipe = null;
            if (!await Ad.MenuService.UpdateLunchEntry(menuEntry)) MessageBox.Show("Kunde inte radera maträtt för lunch.");
        }
        [RelayCommand]
        public async Task RemoveDinner(MenuEntry menuEntry)
        {
            if (menuEntry == null) return;
            menuEntry.DinnerRecipe = null;
            if (!await Ad.MenuService.UpdateDinnerEntry(menuEntry)) MessageBox.Show("Kunde inte radera maträtt för dinner.");

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
            MenuListSource.View.Refresh();  

        }
        [RelayCommand]
        private void NextWeek()
        {
            WeekToDisplay = WeekToDisplay.AddDays(7);
            DisplayWeek();
            MenuListSource.View.Refresh();
        }
        [RelayCommand]
        private void LunchChanged(MenuEntry menuEntry)
        {
            currentEntry = menuEntry;
            lunchordinner = true;
            Ad.CurrentPage = new RecipePage();
        }
        [RelayCommand]
        private void DinnerChanged(MenuEntry menuEntry)
        {
            currentEntry = menuEntry;
            lunchordinner = false;
            Ad.CurrentPage = new RecipePage();
        }


    }


}
