using Matsedeln.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Matsedeln.Usercontrols
{

    public partial class NewRecipeControl : UserControl
    {

        public NewRecipeControl()
        {
            InitializeComponent();
            DataContext = new NewRecipeControlViewModel();
            SetFocus();
        }


        private void SetFocus()
        {
            Dispatcher.BeginInvoke(() => Keyboard.Focus(RecipeNameInput), DispatcherPriority.Input);
        }



        public void CheckIfDigit_preview(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d$"))
            {
                e.Handled = true;
            }
        }

        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox box)
            {
                string newText = box.Text.Insert(box.SelectionStart, e.Text);

                // Allow empty string or a valid decimal number
                e.Handled = !Regex.IsMatch(newText, @"^$|^\d+(\,\d{0,2})?$");
            }
        }

    }
}
