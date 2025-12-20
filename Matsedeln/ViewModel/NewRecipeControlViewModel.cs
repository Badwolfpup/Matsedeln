using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Pages;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
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
        private string recipeName = "";
        [ObservableProperty]
        private bool showRest;
        [ObservableProperty]
        private Recipe newRecipe;
        [ObservableProperty]
        private Ingredient newIngredient;
        [ObservableProperty]
        private string quantity;



        public NewRecipeControlViewModel()
        {
            NewRecipe = new Recipe("");
            NewIngredient = new Ingredient();
            RecipeImage = new BitmapImage(new Uri(NewRecipe.ImagePath));
            isAddingNewIngredient = true;
            WeakReferenceMessenger.Default.Register<AppData.PassGoodsToUCMessage>(this, (r, m) => { NewIngredient.Good = m.good; NewIngredient.AddUnitOptions(); });
            WeakReferenceMessenger.Default.Register<AppData.PasteImageMessage>(this, (r, m) => OnPasteExecuted());
        }


        [RelayCommand]
        private void ShowAddGoodsUserControl()
        {
            Ad.CurrentUserControl = new NewGoodsControl();
            WeakReferenceMessenger.Default.Send(new AppData.ResetBorderMessage());
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

            if (IsAddingNewIngredient)
            {
                NewRecipe.Ingredientlist.Add(NewIngredient);
                NewIngredient = new Ingredient();
                
            }
            else
            {
                NewIngredient = new Ingredient();
                ResetInput();
            }
        }

        [RelayCommand]
        private async Task AddNewRecipe()
        {
            if (string.IsNullOrEmpty(NewRecipe.Name))
            {
                MessageBox.Show("Ange ett namn för receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NewRecipe.Ingredientlist.Count < 2)
            {
                MessageBox.Show("Lägg till minst två ingredienser i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //if (await DBHelper.AddRecipeToDB(NewRecipe))
            //{
            //    if (hasAddedImage) KopieraBild();
            //    IngredientName.Text = "";
            //    ResetInput();
            //    BindadBild.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            //    NewRecipe = new Recipe("");
            //    RecipeName = "";
            //    Ad.SelectedGood = null;
            //}
            //else
            //{
            //    MessageBox.Show("Något gick fel vid tillägget av receptet i databasen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);

            //}
        }




        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient != null) NewRecipe.Ingredientlist.Remove(ingredient);
        }

        [RelayCommand]
        private void SelectIngredient(Ingredient ingredient)
        {
            IsAddingNewIngredient = false;
            NewIngredient.Good = ingredient.Good;
            NewIngredient.AddUnitOptions();
            WeakReferenceMessenger.Default.Send(new AppData.MoveBorderMessage(ingredient.Good));
            Quantity = ingredient.Quantity.ToString();
        }


        [RelayCommand]
        private void AbortNewRecipe()
        {
            ResetInput();
            NewRecipe = new Recipe("");
            RecipeImage = new BitmapImage(new Uri(NewRecipe.ImagePath));
            NewIngredient = new Ingredient();
            WeakReferenceMessenger.Default.Send(new AppData.ResetBorderMessage());
        }

        public void ResetInput()
        {
            Quantity = "";
            IsAddingNewIngredient = true;
            WeakReferenceMessenger.Default.Send(new AppData.ResetBorderMessage());
        }

        partial void OnRecipeNameChanged(string value)
        {
            ShowRest = !string.IsNullOrEmpty(value);
            NewRecipe.Name = RecipeName;
        }

        partial void OnQuantityChanged(string value)
        {
            NewIngredient.Quantity = int.TryParse(value, out int result) ? result : 0;
            NewIngredient.GetQuantityInGram(NewIngredient.Quantity);
            NewIngredient.ConvertToOtherUnits(NewIngredient.QuantityInGram);
        }

        partial void OnIsAddingNewIngredientChanged(bool value)
        {
            AddingButtonText = value ? "Lägg till ingrediens" : "Uppdatera ingrediens";
        }

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool skaKopieraBild { get; set; } //Styr om bilden ska kopieras eller inte.
        private bool hasAddedImage { get; set; }
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
                        RecipeImage =bitmapImage;
                        hasAddedImage = true;
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
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Bilder\"
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
                    NewRecipe.ImagePath = filgenväg;
                    tempBild = img;
                    RecipeImage = img;
                    hasAddedImage = true;
                    hasExtension = true;
                    AddImagePathToRecipe();
                }
            }
        }

        private void AddImagePathToRecipe()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = _folderpath + @"\Bilder\";
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
