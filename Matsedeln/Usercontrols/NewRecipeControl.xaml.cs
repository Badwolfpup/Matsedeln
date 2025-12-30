using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Utils;
using Matsedeln.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Matsedeln.Usercontrols
{

    public partial class NewRecipeControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public AppData Ad { get; } = AppData.Instance;

        public NewRecipeControl()
        {
            InitializeComponent();
            DataContext = new NewRecipeControlViewModel();
            WeakReferenceMessenger.Default.Register<AppData.PasteImageRecipeUCMessage>(this, (r, m) => BoundImage.Source = m.image);
            SetFocus();
        }

        public NewRecipeControl(Recipe recipe)
        {
            InitializeComponent();
            DataContext = new NewRecipeControlViewModel(recipe);
            SetFocus();
        }

        private void SetFocus()
        {
            Dispatcher.BeginInvoke(() => Keyboard.Focus(RecipeNameInput), DispatcherPriority.Input);
        }

   

        public void CheckIfDigit_preview(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d$"))
            {
                e.Handled = true;
            }
        }

        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox box)
            {
                string newText = box.Text.Insert(box.SelectionStart, e.Text);

                // Allow empty string or a valid decimal number
                e.Handled = !Regex.IsMatch(newText, @"^$|^\d+(\,\d{0,2})?$");
            }
        }

    }
}
