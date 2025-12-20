using Matsedeln.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Matsedeln.Utils;
using Matsedeln.Models;
using Matsedeln.ViewModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for NewGoodsControl.xaml
    /// </summary>
    public partial class NewGoodsControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public AppData Ad { get; } = AppData.Instance;
        public NewGoodsControl()
        {
            InitializeComponent();
            DataContext = new NewGoodsControlViewModel();
        }

        public NewGoodsControl(Goods good)
        {
            InitializeComponent();
            DataContext = new NewGoodsControlViewModel();

        }

        public void CheckIfDigit_preview(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d$"))
            {
                e.Handled = true;
            }
        }

    }
}
