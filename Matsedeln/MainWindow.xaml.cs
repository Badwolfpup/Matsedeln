using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
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

namespace Matsedeln
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public AppData Ad { get; } = AppData.Instance;





        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            LoadDB();
        }

        private async void LoadDB()
        {
            await Ad.LoadDataFromDB();
        }

        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new AppData.PasteImageMessage());

        }




    }
}