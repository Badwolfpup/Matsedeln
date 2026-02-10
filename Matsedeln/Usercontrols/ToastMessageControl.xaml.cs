using System.Windows;
using System.Windows.Controls;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for ToastMessage.xaml
    /// </summary>
    public partial class ToastMessageControl : UserControl
    {
        public ToastMessageControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ToastMessageControl), new PropertyMetadata(null));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(ToastMessageControl), new PropertyMetadata(false));

        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }
    }
}
