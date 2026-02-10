using Matsedeln.ViewModel;
using System.Windows.Controls;


namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class WeeklyMenuControl : UserControl
    {
        public WeeklyMenuControl()
        {
            InitializeComponent();
            DataContext = new WeeklyMenuControlViewModel();
        }
    }
}
