using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Matsedeln.ViewModel
{
    public partial class NewRecipeControlViewModel : ObservableObject
    {
        public AppData Ad { get; } = AppData.Instance;

        [ObservableProperty]
        private bool isAddingNewIngredient = true;
        [ObservableProperty]
        private ImageSource recipeImage;
        [ObservableProperty]
        private string addingButtonText = "Lägg till ingrediens";
        [ObservableProperty]
        private string addingRecipeButtonText = "Lägg till recept";
        [ObservableProperty]
        private string recipeName = "";
        [ObservableProperty]
        private bool showRest;
        [ObservableProperty]
        private Recipe newRecipe;
        [ObservableProperty]
        private Ingredient newIngredient;
        [ObservableProperty]
        private string quantity;
        private bool _shouldCopyImage;
        [ObservableProperty]
        private bool hasAddedImage;
        private bool isAddingNewRecipe = true;

        public NewRecipeControlViewModel(Recipe recipe)
        {
            NewRecipe = recipe;
            RecipeName = recipe.Name;
            HasAddedImage = true;
            isAddingNewRecipe = false;
            AddingRecipeButtonText = "Uppdatera recept";
            RecipeImage = new BitmapImage(new Uri(recipe.ImagePath));
            IsAddingNewIngredient = false;
            WeakReferenceMessenger.Default.Register<AppData.PassGoodsToUCMessage>(this, (r, m) => PassGoodsToUC(m.good));
            WeakReferenceMessenger.Default.Register<AppData.IsGoodAddedToIngredientMessage>(this, (r, m) => IsGoodAddedToIngredient(m.Goods));
            WeakReferenceMessenger.Default.Register<AppData.AddRecipeToRecipeMessage>(this, (r, m) => AddRecipe(m.recipe));
        }

        public NewRecipeControlViewModel()
        {
            NewRecipe = new Recipe("");
            NewIngredient = new Ingredient();
            RecipeImage = new BitmapImage(new Uri(NewRecipe.ImagePath));
            isAddingNewIngredient = true;
            WeakReferenceMessenger.Default.Register<AppData.PassGoodsToUCMessage>(this, (r, m) => PassGoodsToUC(m.good));
            WeakReferenceMessenger.Default.Register<AppData.PasteImageMessage>(this, (r, m) => OnPasteExecuted());
            WeakReferenceMessenger.Default.Register<AppData.IsGoodAddedToIngredientMessage>(this, (r, m) => IsGoodAddedToIngredient(m.Goods));
            WeakReferenceMessenger.Default.Register<AppData.AddRecipeToRecipeMessage>(this, (r, m) => AddRecipe(m.recipe));
        }
        
        private void IsGoodAddedToIngredient(Goods good)
        {
            if (!NewRecipe.Ingredientlist.Any(x => x.Good.Id == good.Id))
            {
                WeakReferenceMessenger.Default.Send(new AppData.RemoveHighlightBorderMessage(good));
            }
        }

        private void PassGoodsToUC(Goods good)
        {
            NewIngredient = new Ingredient();
            ResetInput();
            NewIngredient.Good = good;
            NewIngredient.Unit = "g";
            NewIngredient.AddUnitOptions();
        }

        [RelayCommand]
        private void ShowAddGoodsUserControl()
        {
            Ad.CurrentUserControl = new NewGoodsControl();
            WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
        }

        [RelayCommand]
        private void AddIngredient()
        {
            if (Ad.CurrentPage is not IngredientPage)
            {
                Ad.CurrentPage = Ad.IngredientPageInstance;
                return;
            }
            if (NewIngredient.Good == null)
            {
                MessageBox.Show("Välj en ingrediens i listan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(Quantity) || NewIngredient.Quantity <= 0)
            {
                MessageBox.Show("Ange mängd för ingrediensen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsAddingNewIngredient && NewRecipe.Ingredientlist.Any(x => x.Good.Name == NewIngredient.Good.Name))
            {
                MessageBox.Show("Ingrediensen är redan tillagd i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsAddingNewIngredient)
            {
                NewRecipe.Ingredientlist.Add(NewIngredient);
                NewIngredient = new Ingredient();
                ResetInput();
            }
            else
            {
                NewIngredient = new Ingredient();
                //ResetInput();
            }
        }
        [RelayCommand]
        private void AddRecipe(Recipe recipe)
        {
            if (Ad.CurrentPage is not RecipePage)
            {
                Ad.CurrentPage = Ad.RecipePageInstance;
                return;
            }
            if (!NewRecipe.ChildRecipes.Any(x => x.ChildRecipeId == recipe.Id) && NewRecipe != recipe) NewRecipe.ChildRecipes.Add(new RecipeHierarchy(NewRecipe, recipe));
        }

        [RelayCommand]
        private async Task AddNewRecipe()
        {
            if (string.IsNullOrEmpty(NewRecipe.Name))
            {
                MessageBox.Show("Ange ett namn för receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NewRecipe.Ingredientlist.Count + NewRecipe.ChildRecipes.Count < 2)
            {
                MessageBox.Show("Lägg till minst två saker i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (isAddingNewRecipe)
            {
                if (await Ad.RecipeService.AddRecipe(NewRecipe, Ad.RecipesList))
                {
                    if (_shouldCopyImage) KopieraBild();

                    ResetInput();
                    NewRecipe = new Recipe("");
                    RecipeImage = new BitmapImage(new Uri(NewRecipe.ImagePath));
                    RecipeName = "";
                    WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
                }
                else
                {
                    MessageBox.Show("Något gick fel vid tillägget av receptet i databasen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (await Ad.RecipeService.UpdateRecipe(NewRecipe))
                {
                    MessageBox.Show("Recept uppdaterad!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    WeakReferenceMessenger.Default.Send(new AppData.RemoveHighlightRecipeMessage());
                }
                else
                {
                    MessageBox.Show("Något gick fel vid uppdateringen av receptet i databasen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }




        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient == null)  return;
            NewRecipe.Ingredientlist.Remove(ingredient);
            WeakReferenceMessenger.Default.Send(new AppData.RemoveHighlightBorderMessage(ingredient.Good));
        }

        [RelayCommand]
        private void RemoveRecipe(RecipeHierarchy recipe) => NewRecipe.ChildRecipes.Remove(recipe);  


        [RelayCommand]
        private void SelectIngredient(Ingredient ingredient)
        {
            IsAddingNewIngredient = false;
            NewIngredient = ingredient;
            Quantity = ingredient.Quantity.ToString();
        }


        [RelayCommand]
        private void AbortNewRecipe()
        {
            NewRecipe = new Recipe("");
            RecipeImage = new BitmapImage(new Uri(NewRecipe.ImagePath));
            NewIngredient = new Ingredient();
            ResetInput();
            WeakReferenceMessenger.Default.Send(new AppData.ResetBorderMessage());
        }

        public void ResetInput()
        {
            Quantity = "";
            IsAddingNewIngredient = true;
        }

        partial void OnRecipeNameChanged(string value)
        {
            ShowRest = !string.IsNullOrEmpty(value) && HasAddedImage;
            NewRecipe.Name = RecipeName;
        }

        partial void OnHasAddedImageChanged(bool value)
        {
            ShowRest = !string.IsNullOrEmpty(RecipeName) && HasAddedImage;
        }

        partial void OnQuantityChanged(string value)
        {
            var converter = new ToSweDecimalConverter();
            var parsed = converter.ConvertBack(value, typeof(double), null, CultureInfo.CurrentCulture);
            if (parsed != DependencyProperty.UnsetValue)
            {
                //bool success = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result);
                NewIngredient.Quantity = (double)parsed;  // Or handle as needed
                NewIngredient.GetQuantityInGram(NewIngredient.Quantity);
                NewIngredient.ConvertToOtherUnits();
            }
            else
            {
                NewIngredient.Quantity = 0;  // Invalid input
            }
        }

        partial void OnIsAddingNewIngredientChanged(bool value)
        {
            AddingButtonText = value ? "Lägg till ingrediens" : "Uppdatera ingrediens";
        }

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool skaKopieraBild { get; set; } //Styr om bilden ska kopieras eller inte.
        
        private bool hasExtension { get; set; }
        private BitmapImage tempBild { get; set; } = new BitmapImage();

        [RelayCommand]
        public void OnPasteExecuted()
        {
            if (Clipboard.ContainsImage())
            {
                BitmapImage bitmapImage = new BitmapImage();

                BitmapSource imageSource = Clipboard.GetImage();
                if (imageSource != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Encode BitmapSource to memory stream
                        BitmapEncoder encoder = new PngBitmapEncoder(); // Change encoder type if needed
                        encoder.Frames.Add(BitmapFrame.Create(imageSource));
                        encoder.Save(memoryStream);

                        // Set memory stream position to beginning
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        tempBild = bitmapImage;
                        RecipeImage = bitmapImage;
                        HasAddedImage = true;
                        _shouldCopyImage = true;
                        hasExtension = false;
                        AddImagePathToRecipe();
                    }
                }
            }
        }
        [RelayCommand]
        private void ImageMouseDown()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder")
            };
            open.Multiselect = false;
            if (open.ShowDialog() == true)
            {
                string filgenväg = open.FileName;

                fileextension = System.IO.Path.GetExtension(filgenväg);
                if (System.IO.Path.GetExtension(filgenväg) == ".jpg" || System.IO.Path.GetExtension(filgenväg) == ".jpeg" || System.IO.Path.GetExtension(filgenväg) == ".png")
                {

                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(filgenväg);
                    img.EndInit();
                    RecipeImage = img;
                    string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Bilder");
                    _shouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;

                    HasAddedImage = true;
                    hasExtension = true;
                    if (_shouldCopyImage) AddImagePathToRecipe();
                    else NewRecipe.ImagePath = filgenväg;
                }
            }
        }

        private void AddImagePathToRecipe()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            NewRecipe.ImagePath = filePath;
        }

        public void KopieraBild()
        {


            if (tempBild != null)
            {
                // Create a new BitmapEncoder
                BitmapEncoder encoder = new PngBitmapEncoder(); // Choose the appropriate encoder based on your requirements

                // Create a new MemoryStream to hold the encoded image data
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Encode the BitmapImage and write the encoded data to the MemoryStream
                    encoder.Frames.Add(BitmapFrame.Create(tempBild));
                    encoder.Save(memoryStream);


                    // Write the encoded data from the MemoryStream to the file
                    File.WriteAllBytes(NewRecipe.ImagePath, memoryStream.ToArray());
                }
            }

        }

        #endregion
    }
}
