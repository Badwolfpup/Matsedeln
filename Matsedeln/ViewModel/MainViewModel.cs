using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using System.Windows.Controls;


namespace Matsedeln
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Page currentPage;
        [ObservableProperty]
        private UserControl currentUserControl;

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<ChangePageMessenger>(this, (r, m) => ChangePage(m.Page));
            WeakReferenceMessenger.Default.Register<ChangeUsercontrolMessenger>(this, (r, m) => ChangeUserControl(m.Control));
            CurrentPage = new IngredientPage();
            CurrentUserControl = new NewGoodsControl();
        }



        private void ChangePage(PageType page)
        {
            CurrentPage = page switch
            {
                PageType.Goods => new IngredientPage(),
                PageType.Recipe => new RecipePage(),
                PageType.Menu => new MenuPage(),
                PageType.Dishes when CurrentPage is not DishesPage => new DishesPage(),
                _ => CurrentPage
            };
        }

        private void ChangeUserControl(UserControlType control)
        {
            CurrentUserControl = control switch
            {
                UserControlType.Goods => new NewGoodsControl(),
                UserControlType.Recipe => new NewRecipeControl(),
                UserControlType.Shopping => new ShoppingListControl(),
                UserControlType.Menu => new WeeklyMenuControl(),
                _ => CurrentUserControl
            };
        }

        partial void OnCurrentUserControlChanging(UserControl value)
        {
            if (CurrentUserControl?.DataContext is IDisposable disposable) disposable.Dispose();

            if (CurrentUserControl != null) CurrentUserControl.DataContext = null;
        }

        partial void OnCurrentPageChanging(Page value)
        {

            if (CurrentPage?.DataContext is IDisposable disposable) disposable.Dispose();

            if (CurrentPage != null) CurrentPage.DataContext = null;

        }

    }

}
