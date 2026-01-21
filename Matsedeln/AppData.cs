using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Matsedeln.Pages;
using Matsedeln.Utils;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        public ObservableCollection<MenuEntry> MenuList { get; set; } = new ObservableCollection<MenuEntry>();
        
        
        [ObservableProperty]
        private Page currentPage;
        [ObservableProperty]
        private UserControl currentUserControl;


        #region Messenger records
        public record SelectedBorderMessage(Border Border, bool GoodsOrRecipe);
        public record MoveBorderMessage(Goods Goods);

        public record RemoveHighlightBorderMessage(Goods Goods);
        public record RemoveAllHighlightBorderMessage();
        public record IsGoodAddedToIngredientMessage(Goods Goods);
        public record RefreshCollectionViewMessage();
        public record RefreshIngredientCollectionViewMessage(ObservableCollection<Ingredient> Ingredients);
        public record RefreshMenuEntrySourceMessage();
        public record ResetBorderMessage();

        public record PasteImageGoodsUCMessage(BitmapImage image);
        public record PasteImageRecipeUCMessage(BitmapImage image);

        public record AddRecipeToRecipeMessage(Recipe recipe);

        public record RemoveHighlightRecipeMessage();
        public record GoBackToShoppingListMessage();
        public record ResetShoppinglistUCMessages();
        public record RemoveIngredientShoplistMessage(Recipe recipe);

        public record RefreshIngredientMessage(Recipe recipe);
        public record AddIngredientShopListMessage(Recipe recipe);

        public record ShowShoppingListMessage();
        public record PassGoodsToUCMessage(Goods good);

        public record AddRecipeToMenuMessage(Recipe recipe);
        public record PasteImageMessage();

        #endregion


        public async Task LoadDataFromDB()
        {
            try
            {
                var api = new ApiService();
                var goods = await api.GetListAsync<Goods>("api/goods");
                if (goods != null) GoodsList = new ObservableCollection<Goods>(goods);
                api = new ApiService();
                var recipe = await api.GetListAsync<Recipe>("api/recipe");
                if (recipe != null) RecipesList = new ObservableCollection<Recipe>(recipe);
                api = new ApiService();
                var menu = await api.GetListAsync<MenuEntry>($"api/menuentry/{DateTime.Now}");
                if (menu != null) MenuList = new ObservableCollection<MenuEntry>(menu);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ett fel uppstod vid inläsning av data från databasen: " + ex);
            }
        }
    }
}
