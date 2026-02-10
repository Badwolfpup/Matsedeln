using Matsedeln.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for NewGoodsControl.xaml
    /// </summary>
    public partial class NewGoodsControl : UserControl
    {
        public NewGoodsControl()
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
