using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Matsedeln
{
    public sealed partial class AppData: ObservableObject
    {
        private static AppData _instance;
        private static readonly object _lock = new object();
        private AppData() { }  // Private constructor
        public static AppData Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new AppData();
                }
            }
        }

        public ObservableCollection<Goods> GoodsList { get; set; } = new ObservableCollection<Goods>();
        public ObservableCollection<Recipe> RecipesList { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Ingredient> ShoppingList { get; set; } = new ObservableCollection<Ingredient>();
        public ObservableCollection<MenuItem> MenuList { get; set; } = new ObservableCollection<MenuItem>();
        
        private IngredientPage _ingredientPageInstance;
        public IngredientPage IngredientPageInstance => _ingredientPageInstance ??= new IngredientPage();

        private RecipePage _recipePageInstance;
        public RecipePage RecipePageInstance => _recipePageInstance ??= new RecipePage();

        private MenuPage _menuPageInstance;
        public MenuPage MenuPageInstance => _menuPageInstance ??= new MenuPage();

        public GoodsService GoodsService { get; } = new GoodsService();

        [ObservableProperty]
        private bool isFilterTextboxEnabled = true;
        [ObservableProperty]
        private Page currentPage;
        [ObservableProperty]
        private UserControl currentUserControl;


        #region Messenger records
        public record SelectedBorderMessage(Border Border);
        public record MoveBorderMessage(Goods Goods);


        public record RefreshCollectionViewMessage();

        public record ResetBorderMessage();

        public record PasteImageGoodsUCMessage(BitmapImage image);
        public record PasteImageRecipeUCMessage(BitmapImage image);

        public record RemoveIngredientShoplistMessage(Recipe recipe);

        public record AddIngredientShopListMessage(Recipe recipe);

        public record PassGoodsToUCMessage(Goods good);

        public record PasteImageMessage();
        #endregion
        [ObservableProperty]
        private string filterText = string.Empty;

        public async Task LoadDataFromDB()
        {
            try
            {
                GoodsList = await GoodsService.GetGoods();

                //RecipesList = await DBHelper.GetAllRecipesFromDB(GoodsList);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ett fel uppstod vid inläsning av data från databasen: " + ex);
            }
        }
    }
}
