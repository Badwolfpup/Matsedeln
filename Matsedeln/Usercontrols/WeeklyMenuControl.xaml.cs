using Matsedeln.Utils;
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
    public partial class WeeklyMenuControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private int weeknumber;
        private string weekanddate = "";

        public string Weekanddate
        {
            get => weekanddate;
            set 
            {
                if (weekanddate != value)
                {
                    weekanddate = value;
                    OnPropertyChanged(nameof(Weekanddate));
                }
            }
        }

        public ObservableCollection<MenuItem> MenuList { get; set; }
        public CollectionViewSource MenuListSource { get; set; }

        private CultureInfo culture = new CultureInfo("sv-SE");
        private System.Globalization.Calendar calendar;
        private DateTime WeekToDisplay = DateTime.Today;
        

        public WeeklyMenuControl()
        {
            InitializeComponent();
            DataContext = this;
            calendar = culture.Calendar;
            DisplayWeek();
            GetMenuListFromDB();
        }

        private async void  GetMenuListFromDB()
        {
            MenuList = await DBHelper.GetAllMenuItemsFromDB();
            MenuListSource = new CollectionViewSource();
            MenuListSource.Source = MenuList;
        }

        private void DisplayWeek()
        {
            int diff = (7 + (WeekToDisplay.DayOfWeek - DayOfWeek.Monday)) % 7;
            weeknumber = calendar.GetWeekOfYear(WeekToDisplay, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
            Weekanddate = $"{WeekToDisplay.Year} - Vecka {weeknumber}: {WeekToDisplay.AddDays(-diff).Date.ToString("d/M", CultureInfo.InvariantCulture)} - {WeekToDisplay.AddDays(-diff + 6).Date.ToString("d/M", CultureInfo.InvariantCulture)}";
        }

        private void PrevWeek_Click(object sender, RoutedEventArgs e)
        {
            WeekToDisplay = WeekToDisplay.AddDays(-7);
            DisplayWeek();

        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            WeekToDisplay = WeekToDisplay.AddDays(7);
            DisplayWeek();
        }

        private void Lunch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Dinner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
