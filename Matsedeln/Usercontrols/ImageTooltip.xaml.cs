using System.Windows;
using System.Windows.Controls;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for ImageTooltip.xaml
    /// </summary>
    public partial class ImageTooltip : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageTooltip), new PropertyMetadata(null));

        public static readonly DependencyProperty RecipeNameProperty =
          DependencyProperty.Register("RecipeName", typeof(string), typeof(ImageTooltip), new PropertyMetadata(null));

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public string RecipeName
        {
            get { return (string)GetValue(RecipeNameProperty); }
            set { SetValue(RecipeNameProperty, value); }
        }

        public ImageTooltip()
        {
            InitializeComponent();
        }
    }
}
