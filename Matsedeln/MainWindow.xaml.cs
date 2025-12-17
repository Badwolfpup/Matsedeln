using Matsedeln;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;

namespace Matsedeln
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        private bool enableaddgoodsbtn = true;
        private bool enableaddrecipebtn = true;

        private bool enablegoodsbtn = false;
        private bool enablerecipebtn = true;
        private bool showgoodsusercontrol = false;
        private bool showrecipeusercontrol = false;
        private SolidColorBrush showaddgoodsbtncolor = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        private SolidColorBrush showaddrecipebtncolor = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        private SolidColorBrush showviewrecipebtncolor = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        private SolidColorBrush showviewshoppinglistbtncolor = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        private SolidColorBrush showgoodsbtncolor = new SolidColorBrush(Color.FromRgb(105, 105, 105));
        private SolidColorBrush showrecipebtncolor = new SolidColorBrush(Color.FromRgb(0, 120, 212));
        private Goods selectedgood;

        public bool ShowGoodsUsercontrol
        {
            get { return showgoodsusercontrol; }
            set
            {
                if (showgoodsusercontrol != value)
                {
                    showgoodsusercontrol = value;
                    OnPropertyChanged(nameof(ShowGoodsUsercontrol));
                }
            }
        }

        public bool ShowRecipeUsercontrol
        {
            get { return showrecipeusercontrol; }
            set
            {
                if (showrecipeusercontrol != value)
                {
                    showrecipeusercontrol = value;
                    OnPropertyChanged(nameof(ShowRecipeUsercontrol));
                }
            }
        }

        public bool EnableAddGoodsBtn
        {
            get { return enableaddgoodsbtn; }
            set
            {
                if (enableaddgoodsbtn != value)
                {
                    enableaddgoodsbtn = value;
                    ChangeBtnColor();
                    OnPropertyChanged(nameof(EnableAddGoodsBtn));
                }
            }
        }
        public bool EnableAddRecipeBtn
        {
            get { return enableaddrecipebtn; }
            set
            {
                if (enableaddrecipebtn != value)
                {
                    enableaddrecipebtn = value;
                    ChangeBtnColor();
                    OnPropertyChanged(nameof(EnableAddRecipeBtn));
                }
            }
        }

  
        public bool EnableGoodsBtn
        {
            get { return enablegoodsbtn; }
            set
            {
                if (enablegoodsbtn != value)
                {
                    enablegoodsbtn = value;
                    ChangeBtnColor();
                    OnPropertyChanged(nameof(EnableGoodsBtn));
                }
            }
        }

        public bool EnableRecipeBtn
        {
            get { return enablerecipebtn; }
            set
            {
                if (enablerecipebtn != value)
                {
                    enablerecipebtn = value;
                    ChangeBtnColor();
                    OnPropertyChanged(nameof(EnableRecipeBtn));
                }
            }
        }

        public SolidColorBrush ShowAddsBtnColor
        {
            get { return showaddgoodsbtncolor; }
            set
            {
                if (showaddgoodsbtncolor != value)
                {
                    showaddgoodsbtncolor = value;
                    OnPropertyChanged(nameof(ShowAddsBtnColor));
                }
            }
        }

        public SolidColorBrush ShowAddRecipeBtnColor
        {
            get { return showaddrecipebtncolor; }
            set
            {
                if (showaddrecipebtncolor != value)
                {
                    showaddrecipebtncolor = value;
                    OnPropertyChanged(nameof(ShowAddRecipeBtnColor));
                }
            }
        }

        public SolidColorBrush ShowGoodsBtnColor
        {
            get { return showgoodsbtncolor; }
            set
            {
                if (showgoodsbtncolor != value)
                {
                    showgoodsbtncolor = value;
                    OnPropertyChanged(nameof(ShowGoodsBtnColor));
                }
            }
        }

        public SolidColorBrush ShowRecipeBtnColor
        {
            get { return showrecipebtncolor; }
            set
            {
                if (showrecipebtncolor != value)
                {
                    showrecipebtncolor = value;
                    OnPropertyChanged(nameof(ShowRecipeBtnColor));
                }
            }
        }

        public SolidColorBrush ShowViewRecipeBtnColor
        {
            get { return showviewrecipebtncolor; }
            set
            {
                if (showviewrecipebtncolor != value)
                {
                    showviewrecipebtncolor = value;
                    OnPropertyChanged(nameof(ShowViewRecipeBtnColor));
                }
            }
        }

        public SolidColorBrush ShowViewShoppinglistBtnColor
        {
            get { return showviewshoppinglistbtncolor; }
            set
            {
                if (showviewshoppinglistbtncolor != value)
                {
                    showviewshoppinglistbtncolor = value;
                    OnPropertyChanged(nameof(ShowViewShoppinglistBtnColor));
                }
            }
        }

        public Goods SelectedGood
        {
            get { return selectedgood; }
            set
            {
                if (selectedgood != value)
                {
                    selectedgood = value;
                    OnPropertyChanged(nameof(SelectedGood));
                }
            }
        }

        private ObservableCollection<Goods> goodsList;
        public ObservableCollection<Goods> GoodsList
        {
            get { return goodsList; }
            set
            {
                if (goodsList != value)
                {
                    goodsList = value;
                    OnPropertyChanged(nameof(GoodsList));
                }
            }
        }

        private ObservableCollection<Recipe> recipesList;

        public ObservableCollection<Recipe> RecipesList
        {
            get { return recipesList; }
            set
            {
                if (recipesList != value)
                {
                    recipesList = value;
                    OnPropertyChanged(nameof(RecipesList));
                }
            }
        }

        private ObservableCollection<Ingredient> shoppingList;
        public ObservableCollection<Ingredient> ShoppingList
        {
            get { return shoppingList; }
            set
            {
                if (shoppingList != value)
                {
                    shoppingList = value;
                    OnPropertyChanged(nameof(ShoppingList));
                }
            }
        }

        public CollectionViewSource GoodsViewSource { get; set; }
        public CollectionViewSource RecipesViewSource { get; set; }

        public IngredientPage IngredientPageInstance { get; set; }
        public RecipePage RecipePageInstance { get; set; }
        #endregion
        private void ChangeBtnColor()
        {
            ShowAddsBtnColor = EnableAddGoodsBtn ? new SolidColorBrush(Color.FromRgb(0, 120, 212)) : new SolidColorBrush(Color.FromRgb(105, 105, 105));
            ShowAddRecipeBtnColor = EnableAddRecipeBtn ? new SolidColorBrush(Color.FromRgb(0, 120, 212)) : new SolidColorBrush(Color.FromRgb(105, 105, 105));
            ShowGoodsBtnColor = EnableGoodsBtn ? new SolidColorBrush(Color.FromRgb(0, 120, 212)) : new SolidColorBrush(Color.FromRgb(105, 105, 105));
            ShowRecipeBtnColor = EnableRecipeBtn ? new SolidColorBrush(Color.FromRgb(0, 120, 212)) : new SolidColorBrush(Color.FromRgb(105, 105, 105));
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            IngredientPageInstance = new IngredientPage(this);
            RecipePageInstance = new RecipePage(this);
            GoodsViewSource = new CollectionViewSource();
            RecipesViewSource = new CollectionViewSource();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDB();
        }

        private async void LoadDataFromDB()
        {
            try
            {
                GoodsList = await DBHelper.GetAllGoodsFromDB();
                GoodsViewSource.Source = GoodsList;
                GoodsViewSource.Filter += FilterGoods;
                RecipesList = await DBHelper.GetAllRecipesFromDB(GoodsList);
                RecipesViewSource.Source = RecipesList;
                RecipesViewSource.Filter += FilterRecipe;
                EnableGoodsBtn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ett fel uppstod vid inläsning av data från databasen: " + ex);
            }
        }

        private void FilterGoods(object sender, FilterEventArgs e)
        {
            if (e.Item is Goods good)
            {
                if (!string.IsNullOrEmpty(FilterTextbox.Text))
                {
                    if (good.Name.IndexOf(FilterTextbox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = true;
                }
            }
        }

        private void FilterRecipe(object sender, FilterEventArgs e)
        {
            if (e.Item is Recipe recipe)
            {
                if (!string.IsNullOrEmpty(FilterTextbox.Text))
                {
                    if (recipe.Name.IndexOf(FilterTextbox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = true;
                }
            }
        }

        private void OpenAddIngredient_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowIngredients_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                ShowGoodsUsercontrol = true;
                ShowRecipeUsercontrol = false;
                if (AddContentControl != null && AddContentControl.Content is ShoppingListControl shop)
                {
                    shop.ShowIngredients = false;
                    shop.ShowShoppinglist = false;
                }
                EnableAddGoodsBtn = true;
                EnableAddRecipeBtn = true;
                FilterTextbox.Text = string.Empty;
                GoodsViewSource.View.Refresh();
                ContentFrame.Navigate(IngredientPageInstance);
                EnableGoodsBtn = false;
                EnableRecipeBtn = true;
            }
        }

        private void ShowRecipes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                ShowRecipeUsercontrol = true;
                ShowGoodsUsercontrol = false;
                EnableRecipeBtn = false;
                FilterTextbox.Text = string.Empty;
                RecipesViewSource.View.Refresh();
                ContentFrame.Navigate(RecipePageInstance);
                AddContentControl.Content = new ShoppingListControl(this);
                EnableGoodsBtn = true;

            }
        }


        private void ShowAddGoods_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EnableAddGoodsBtn = false;
                AddContentControl.Content = new NewGoodsControl(this);
                EnableAddRecipeBtn = true;
            }
        }

        private void ShowAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EnableAddRecipeBtn = false;
                AddContentControl.Content = new NewRecipeControl(this, RecipesList);
                EnableAddGoodsBtn = true;
            }
        }

        private void ShowViewRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EnableAddGoodsBtn = false;
                AddContentControl.Content = new NewGoodsControl(this);
                EnableAddRecipeBtn = true;
            }
        }

        private void ShowViewShoppinglist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                EnableAddRecipeBtn = false;
                AddContentControl.Content = new NewRecipeControl(this, RecipesList);
                EnableAddGoodsBtn = true;
            }
        }

        private void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (AddContentControl.Content != null && AddContentControl.Content is NewGoodsControl ngc)
            {
                ngc.OnPasteExecuted(sender, e);
            }
            else if (AddContentControl.Content != null && AddContentControl.Content is NewRecipeControl nrc)
            {
                nrc.OnPasteExecuted(sender, e);
            }
        }

        private void FilterTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EnableGoodsBtn) RecipesViewSource.View.Refresh();
            else if (EnableRecipeBtn) GoodsViewSource.View.Refresh();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content != null && ContentFrame.Content is IngredientPage)
            {
                if (SelectedGood == null)
                {
                    MessageBox.Show("Du måste välja något innan du kan radera.");
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Vill du verkligen radera denna vara?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (!await DBHelper.DeleteGoodsFromDB(SelectedGood))
                    {
                        MessageBox.Show("Det gick inte att radera varan");
                        return;
                    }
                    GoodsList.Remove(SelectedGood);
                    GoodsViewSource.View.Refresh();
                }
            } 
            else
            {
                var allBorders = FindAllBorders(RecipePageInstance.RecipeItemsControl);
                var sum = allBorders.Sum(x => x.BorderBrush == Brushes.Red ? 1 : 0);
                if (sum == 0)
                {
                    MessageBox.Show("Du måste välja något innan du kan radera.");
                    return;
                }
                else if (sum > 1)
                {
                    MessageBox.Show("Du får bara välja ett recept innan du kan radera.");
                    return;
                }
                var SelectedBorder = allBorders.FirstOrDefault(x => x.DataContext is Recipe && x.BorderBrush == Brushes.Red);
                if (SelectedBorder is Border b && b.DataContext is Recipe SelectedRecipe)
                {
                    if (SelectedRecipe == null)
                    {
                        MessageBox.Show("Det gick inte att radera. Problem att hitta datacontext");
                        return;
                    }
                    MessageBoxResult result = MessageBox.Show("Vill du verkligen radera detta recept?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (!await DBHelper.DeleteRecipeFromDB(SelectedRecipe))
                        {
                            MessageBox.Show("Det gick inte att radera varan");
                            return;
                        }
                        RecipesList.Remove(SelectedRecipe);
                        RecipesViewSource.View.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Det gick inte att radera. Problem att hitta border");
                    return;
                }
            }
        }
        public IEnumerable<Border> FindAllBorders(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If this child is a Border, yield it
                if (child is Border border)
                {
                    yield return border;
                }

                // Recurse into children
                foreach (var descendant in FindAllBorders(child))
                {
                    yield return descendant;
                }
            }
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            AddContentControl.Content = new WeeklyMenuControl();
        }
    }
}