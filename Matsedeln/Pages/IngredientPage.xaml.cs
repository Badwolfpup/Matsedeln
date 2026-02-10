using Matsedeln.ViewModel;
using System.Windows.Controls;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for IngredientPage.xaml
    /// </summary>
    public partial class IngredientPage : Page
    {

        public IngredientPage()
        {
            InitializeComponent();
            DataContext = new IngredientPageViewModel();
        }



    }
}
