using Matsedeln.ViewModel;
using System.Windows.Controls;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for ShoppingListControl.xaml
    /// </summary>
    public partial class ShoppingListControl : UserControl
    {

        public ShoppingListControl()
        {
            InitializeComponent();
            DataContext = new ShoppingListViewModel();

        }
    }
}
