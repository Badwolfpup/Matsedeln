using Matsedeln.ViewModel;
using System.Windows.Controls;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for RecipePage.xaml
    /// </summary>
    public partial class RecipePage : Page
    {


        public RecipePage()
        {
            InitializeComponent();
            DataContext = new RecipePageViewModel();
        }
    }
}
