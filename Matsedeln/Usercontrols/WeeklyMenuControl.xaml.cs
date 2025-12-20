using Matsedeln.Utils;
using Matsedeln.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class WeeklyMenuControl : UserControl
    {
        public WeeklyMenuControl()
        {
            InitializeComponent();
            DataContext = new WeeklyMenuControlViewModel();
        }
    }
}
