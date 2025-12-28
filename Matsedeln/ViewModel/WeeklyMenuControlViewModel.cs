using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public CollectionViewSource RecipesViewSource { get; set; }
        private CultureInfo culture = new CultureInfo("sv-SE");
        private System.Globalization.Calendar calendar;
        private DateTime WeekToDisplay = DateTime.Today;

        public WeeklyMenuControlViewModel()
        {
            calendar = culture.Calendar;
            DisplayWeek();
            MenuListSource = new CollectionViewSource();
            MenuListSource.Source = Ad.MenuList;
            RecipesViewSource = new CollectionViewSource();
            RecipesViewSource.Source = Ad.RecipesList;
            WeakReferenceMessenger.Default.Register<AppData.AddRecipeToMenuMessage>(this, (r, m) => UpdateMenu(m.recipe));
        }

        private async void UpdateMenu(Recipe recipe)
        {
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
            await Ad.MenuService.AddEmptyDays(WeekToDisplay);
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
