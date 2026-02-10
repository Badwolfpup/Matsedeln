using Matsedeln.ViewModel;
using System.Windows.Controls;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
            DataContext = new MenuPageViewModel();
        }

    }
}
