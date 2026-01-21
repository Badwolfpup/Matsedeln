using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Printing;
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

        [ObservableProperty]
        private bool isAddingNewIngredient = true;
        [ObservableProperty]
        private ImageSource recipeImage;
        [ObservableProperty]
        private string goodOrRecipeButton = "Visa recept";
        [ObservableProperty]
        private string recipeName = "";
        [ObservableProperty]
        private bool showRest;
        [ObservableProperty]
        private RecipeWrapper newRecipe;
        [ObservableProperty]
        private Ingredient newIngredient;
        [ObservableProperty]
        private string quantity;
        [ObservableProperty]
        private bool _shouldCopyImage;
        [ObservableProperty]
        private bool hasAddedImage;
        private bool isAddingNewRecipe = true;
        private bool isEditingRecipe = false;
        private bool showRecipes = true;

        public NewRecipeControlViewModel()
        {
            NewRecipe = new RecipeWrapper();
            NewIngredient = new Ingredient();
            RecipeImage = new BitmapImage(new Uri(NewRecipe.recipe.ImagePath));
            WeakReferenceMessenger.Default.Register<AppData.PasteImageMessage>(this, (r, m) => OnPasteExecuted());
            WeakReferenceMessenger.Default.Register<SelectedGoodsMessenger>(this, (r, m) => LoadIngredient(m.selectedGood.good));
            WeakReferenceMessenger.Default.Register<SelectedRecipeMessenger>(this, (r, m) =>  NewRecipe.recipe = m.recipe);
            WeakReferenceMessenger.Default.Register<AddChildRecipeMessenger>(this, (r, m) =>
            {
                if (!NewRecipe.recipe.ChildRecipes.Any(x => x.ChildRecipeId == m.recipe.Id) && NewRecipe.recipe != m.recipe) NewRecipe.recipe.ChildRecipes.Add(new RecipeHierarchy(NewRecipe.recipe, m.recipe));
            });
        }

        private void LoadIngredient(Goods good)
        {
            
            NewIngredient.Good = good; 
            NewIngredient.Initialize();
            IsAddingNewIngredient = true;
        }

        [RelayCommand]
        private void ShowAddGoodsUserControl()
        {
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger("goods"));
            WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(0));
        }


        [RelayCommand]
        private void AddIngredient()
        {

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
            NewRecipe.Ingredientlist.Add(NewIngredient);
            WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(NewIngredient.Good.Id));
            NewIngredient = new Ingredient();
            ResetInput();
        }

        [RelayCommand]
        private void ChangePage(Recipe recipe)
        {
            if (showRecipes)
            {
                GoodOrRecipeButton = "Visa ingredienser";
                WeakReferenceMessenger.Default.Send(new ChangePageMessenger("recipe"));
                if (NewRecipe.recipe.ChildRecipes.Count > 0)
                {
                    NewRecipe.recipe.ChildRecipes.ToList().ForEach(x => WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(x.ChildRecipe.Id)));
                }
                showRecipes = !showRecipes;
            }
            else
            {
                GoodOrRecipeButton = "Visa recept";
                WeakReferenceMessenger.Default.Send(new ChangePageMessenger("goods"));
                if (NewRecipe.Ingredientlist.Count > 0)
                {
                    NewRecipe.Ingredientlist.ToList().ForEach(x => WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(x.Good.Id)));
                }
                showRecipes = !showRecipes;
            }
        }

        [RelayCommand]
        private async Task AddRecipe()
        {
            if (string.IsNullOrEmpty(NewRecipe.recipe.Name))
            {
                MessageBox.Show("Ange ett namn för receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var message = new NameExistsMessenger(NewRecipe.recipe.Name);
            WeakReferenceMessenger.Default.Send(message);
            if (message.HasReceivedResponse && message.Response == true)
            {
                MessageBox.Show("Det finns redan ett recept med det namnet");
                return;
            }
            if ((NewRecipe.Ingredientlist.Count + NewRecipe.recipe.ChildRecipes.Count) < 2 && NewRecipe.recipe.Name != "Färdigmat")
            {
                MessageBox.Show("Lägg till minst en sak i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var api = new ApiService();
            var serverPath = await api.UploadImageAsync(NewRecipe.recipe.ImagePath);
            if (string.IsNullOrEmpty(serverPath))
            {
                MessageBox.Show("Något gick fel vid uppladdningen av bilden.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NewRecipe.recipe.ImagePath = serverPath;
            var result = await api.PostAsync<Recipe>("api/recipe", NewRecipe.recipe);
            if (result)
            {
                WeakReferenceMessenger.Default.Send(new RefreshListMessenger());
                WeakReferenceMessenger.Default.Send(new ToastMessage("Receptet har lagts till/uppdaterats."));
                ResetUserControl();
                WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(0));
            }
            else
            {
                MessageBox.Show("Något gick fel vid tillägget/uppdateringen av varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }




        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient == null)  return;
            WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(ingredient.Good.Id));
            NewRecipe.Ingredientlist.Remove(ingredient);
        }

        [RelayCommand]
        private void RemoveRecipe(RecipeHierarchy recipe) => NewRecipe.recipe.ChildRecipes.Remove(recipe);  


        [RelayCommand]
        private void SelectIngredient(Ingredient ingredient)
        {
            IsAddingNewIngredient = false;
            NewIngredient = ingredient;
            Quantity = ingredient.Quantity.ToString();
        }


        [RelayCommand]
        private void ResetUserControl()
        {
            NewRecipe = new RecipeWrapper();
            RecipeImage = new BitmapImage(new Uri(NewRecipe.recipe.ImagePath));
            NewIngredient = new Ingredient();
            ResetInput();
            WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(0));
        }

        public void ResetInput()
        {
            Quantity = "";
            IsAddingNewIngredient = true;
        }

        partial void OnRecipeNameChanged(string value)
        {
            ShowRest = !string.IsNullOrEmpty(value) && HasAddedImage;
            NewRecipe.recipe.Name = RecipeName;
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

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool hasExtension { get; set; }
        private BitmapImage tempBild { get; set; } = new BitmapImage();


        public void SetImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // Default to dummy if path is null
                path = "pack://application:,,,/Images/dummybild.png";
            }

            Uri finalUri;

            // 1. Check if it's already a full Pack URI
            if (path.StartsWith("pack://"))
            {
                finalUri = new Uri(path, UriKind.Absolute);
            }
            // 2. Check if it's a relative server path (e.g., /images/...)
            else if (path.StartsWith("/"))
            {
                string baseUrl =
#if DEBUG
                    "https://localhost:7039";
#else
            "https://your-smarterasp-domain.com"; 
#endif
                string fullUrl = $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
                finalUri = new Uri(fullUrl, UriKind.Absolute);
            }
            // 3. Otherwise, assume it's a direct web link or fallback
            else
            {
                finalUri = new Uri(path, UriKind.RelativeOrAbsolute);
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = finalUri;
                // This prevents the UI from locking if you ever replace the image
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Use the [ObservableProperty] generated setter
                RecipeImage = bitmap;
            }
            catch (Exception ex)
            {
                // If everything fails, hard-code the fallback
                RecipeImage = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            }
        }

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
                        ShouldCopyImage = true;
                        hasExtension = false;
                        AddImagePathToGood();
                    }
                }
            }
        }

        [RelayCommand]
        private void ImageMouseDown()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder")
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
                    tempBild = img;
                    RecipeImage = img;
                    string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
                    ShouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;
                    HasAddedImage = true;
                    hasExtension = true;
                    if (ShouldCopyImage) AddImagePathToGood();
                    else NewRecipe.recipe.ImagePath = filgenväg;
                }
            }
        }

        private void AddImagePathToGood()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = System.IO.Path.Combine(_folderpath, "Bilder");
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            NewRecipe.recipe.ImagePath = filePath;
            KopieraBild();
        }

        public void KopieraBild()
        {

            if (!HasAddedImage) return;
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
                    File.WriteAllBytes(NewRecipe.recipe.ImagePath, memoryStream.ToArray());
                }
            }

        }

        #endregion
        //#region Handle Images
        //private string fileextension; //Håller koll på filändelsen på bilden.

        //private bool skaKopieraBild { get; set; } //Styr om bilden ska kopieras eller inte.

        //private bool hasExtension { get; set; }
        //private BitmapImage tempBild { get; set; } = new BitmapImage();

        //[RelayCommand]
        //public void OnPasteExecuted()
        //{
        //    if (Clipboard.ContainsImage())
        //    {
        //        BitmapImage bitmapImage = new BitmapImage();

        //        BitmapSource imageSource = Clipboard.GetImage();
        //        if (imageSource != null)
        //        {
        //            using (MemoryStream memoryStream = new MemoryStream())
        //            {
        //                // Encode BitmapSource to memory stream
        //                BitmapEncoder encoder = new PngBitmapEncoder(); // Change encoder type if needed
        //                encoder.Frames.Add(BitmapFrame.Create(imageSource));
        //                encoder.Save(memoryStream);

        //                // Set memory stream position to beginning
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                bitmapImage.BeginInit();
        //                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmapImage.StreamSource = memoryStream;
        //                bitmapImage.EndInit();
        //                tempBild = bitmapImage;
        //                RecipeImage = bitmapImage;
        //                HasAddedImage = true;
        //                _shouldCopyImage = true;
        //                hasExtension = false;
        //                AddImagePathToRecipe();
        //            }
        //        }
        //    }
        //}
        //[RelayCommand]
        //private void ImageMouseDown()
        //{
        //    OpenFileDialog open = new OpenFileDialog()
        //    {
        //        InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder")
        //    };
        //    open.Multiselect = false;
        //    if (open.ShowDialog() == true)
        //    {
        //        string filgenväg = open.FileName;

        //        fileextension = System.IO.Path.GetExtension(filgenväg);
        //        if (System.IO.Path.GetExtension(filgenväg) == ".jpg" || System.IO.Path.GetExtension(filgenväg) == ".jpeg" || System.IO.Path.GetExtension(filgenväg) == ".png")
        //        {

        //            BitmapImage img = new BitmapImage();
        //            img.BeginInit();
        //            img.UriSource = new Uri(filgenväg);
        //            img.EndInit();
        //            RecipeImage = img;
        //            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Bilder");
        //            _shouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;

        //            HasAddedImage = true;
        //            hasExtension = true;
        //            if (_shouldCopyImage) AddImagePathToRecipe();
        //            else NewRecipe.ImagePath = filgenväg;
        //        }
        //    }
        //}

        //private void AddImagePathToRecipe()
        //{
        //    string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
        //    string bildfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
        //    if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

        //    string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

        //    NewRecipe.ImagePath = filePath;
        //}

        //public void KopieraBild()
        //{


        //    if (tempBild != null)
        //    {
        //        // Create a new BitmapEncoder
        //        BitmapEncoder encoder = new PngBitmapEncoder(); // Choose the appropriate encoder based on your requirements

        //        // Create a new MemoryStream to hold the encoded image data
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            // Encode the BitmapImage and write the encoded data to the MemoryStream
        //            encoder.Frames.Add(BitmapFrame.Create(tempBild));
        //            encoder.Save(memoryStream);


        //            // Write the encoded data from the MemoryStream to the file
        //            File.WriteAllBytes(NewRecipe.ImagePath, memoryStream.ToArray());
        //        }
        //    }

        //}

        //#endregion
    }
}
