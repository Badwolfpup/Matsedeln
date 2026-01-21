using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.ViewModel;
using MatsedelnShared.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Matsedeln
{
    public partial class MainViewModel: ObservableObject
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public AppData Ad { get; } = AppData.Instance;


        #region Properties
        [ObservableProperty]
        private Page currentPage;
        [ObservableProperty]
        private UserControl currentUserControl;
        [ObservableProperty]
        private bool showGoodsUsercontrol = false;
        [ObservableProperty]
        private bool showRecipeUsercontrol = false;
        private Goods selectedGood;
        #endregion

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<ChangePageMessenger>(this, (r, m) => ChangePage(m.TypeOfPage));
            WeakReferenceMessenger.Default.Register<ChangeUsercontrolMessenger>(this, (r, m) => ChangeUserControl(m.TypeOfControl));
            CurrentPage = new IngredientPage();
            CurrentUserControl = new NewGoodsControl();
        }

        private void ChangePage(string page)
        {
            if (page == "goods") CurrentPage = new IngredientPage();
            else if (page == "recipe") CurrentPage = new RecipePage();
            else if (page == "menu") CurrentPage = new MenuPage();
        }

        private void ChangeUserControl(string control)
        {
            if (control == "goods") CurrentUserControl = new NewGoodsControl();
            else if (control == "recipe") CurrentUserControl = new NewRecipeControl();
            else if (control == "shopping") CurrentUserControl = new ShoppingListControl();
            else if (control == "menup") CurrentUserControl = new WeeklyMenuControl(); 
        }
    }

}
