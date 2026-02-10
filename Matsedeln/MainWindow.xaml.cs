using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Usercontrols;
using System.Windows;
using System.Windows.Input;

namespace Matsedeln
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            WeakReferenceMessenger.Default.Register<ToastMessage>(this, async (r, m) =>
            {
                // 1. Create and add the toast
                var toast = new ToastMessageControl { Message = m.Message, IsError = m.IsError };
                ToastContainer.Children.Add(toast);

                // 2. Wait for the duration (this doesn't block the UI thread)
                await Task.Delay(TimeSpan.FromSeconds(m.DurationInSeconds));

                // 3. Remove it (No need for Dispatcher.Invoke because await returns us to the UI thread!)
                if (ToastContainer.Children.Contains(toast))
                {
                    ToastContainer.Children.Remove(toast);
                }
            });
        }



        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new PasteImageMessage());

        }




    }
}