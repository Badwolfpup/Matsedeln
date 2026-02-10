using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Matsedeln.Usercontrols
{
    public partial class LoadingSpinner : UserControl
    {
        public static readonly DependencyProperty SpinnerColorProperty =
            DependencyProperty.Register(
                "SpinnerColor",
                typeof(Brush),
                typeof(LoadingSpinner),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(33, 150, 243))));

        public Brush SpinnerColor
        {
            get { return (Brush)GetValue(SpinnerColorProperty); }
            set { SetValue(SpinnerColorProperty, value); }
        }

        public LoadingSpinner()
        {
            InitializeComponent();
        }
    }
}