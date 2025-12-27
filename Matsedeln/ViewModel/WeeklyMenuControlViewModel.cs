using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matsedeln.Models;
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
            CopyRecipes();
            RecipesViewSource = new CollectionViewSource();
            RecipesViewSource.Source = LocalList;
        }

        private void CopyRecipes()
        {
            LocalList = new ObservableCollection<Recipe>();
            var emptyrecipe = new Recipe { Name = "Ingen rätt är vald" };
            emptyrecipe.Id = 0;
            //emptyrecipe.ImagePath = null;
            LocalList.Add(emptyrecipe);

            foreach (var item in Ad.RecipesList)
            {
                LocalList.Add(new Recipe(item));
            }
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
        private async void LunchSelectionChanged(object[] obj)
        {
            var menuItem = obj[0] as MenuEntry;
            var recipe = obj[1] as Recipe;

            if (menuItem == null || recipe == null) return;
            menuItem.LunchRecipe = recipe;
            if (await Ad.MenuService.UpdateLunchEntry(menuItem)) menuItem.LunchRecipe = recipe;

        }
        [RelayCommand]
        private async void DinnerSelectionChanged(object[] obj)
        {
            var menuItem = obj[0] as MenuEntry;
            var recipe = obj[1] as Recipe;

            if (menuItem == null || recipe == null) return;
            if (await Ad.MenuService.UpdateDinnerEntry(menuItem)) menuItem.DinnerRecipe = recipe;
        }
    }


}
