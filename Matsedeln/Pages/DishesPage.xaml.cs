using Matsedeln.ViewModel;
using System.Windows.Controls;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for DishesPage.xaml
    /// </summary>
    public partial class DishesPage : Page
    {
        public DishesPage()
        {
            InitializeComponent();
            DataContext = new DishesPageViewModel();
        }
    }
}
