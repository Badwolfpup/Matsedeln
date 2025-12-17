using Matsedeln;
using Matsedeln.Pages;
using Matsedeln.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matsedeln.Usercontrols
{

    public partial class NewRecipeControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly MainWindow mainWindow;

        public NewRecipeControl(MainWindow main, ObservableCollection<Recipe> reclist)
        {
            InitializeComponent();
            DataContext = this;
            mainWindow = main;
            recipeList = reclist;
            NewRecipe = new Recipe("");
            BindadBild.Source = NewRecipe.Imagepath != null ? new BitmapImage(new Uri(NewRecipe.Imagepath)) : new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));

            isAddingNewIngredient = true;
        }

        private ObservableCollection<Recipe> recipeList;
        private bool isAddingNewIngredient = true;
        private string addIngButtonText = "Lägg till ingrediens";
        private string recipeName = "";
        private bool showrest;
        private Recipe newRecipe;


        public Recipe NewRecipe
        {
            get { return newRecipe; }
            set
            {
                if (newRecipe != value)
                {
                    newRecipe = value;
                    OnPropertyChanged(nameof(NewRecipe));
                }
            }
        }

        public bool ShowRest
        {
            get { return showrest; }
            set
            {
                if (showrest != value)
                {
                    showrest = value;
                    OnPropertyChanged(nameof(ShowRest));
                }
            }
        }

        public string RecipeName
        {
            get { return recipeName; }
            set
            {
                if (recipeName != value)
                {
                    recipeName = value;
                    ShowRest = recipeName != "";
                    OnPropertyChanged(nameof(RecipeName));
                }
            }
        }

        public bool IsAddingNewIngredient
        {
            get { return isAddingNewIngredient; }
            set
            {
                if (isAddingNewIngredient != value)
                {
                    isAddingNewIngredient = value;
                    AddIngButtonText = isAddingNewIngredient ? "Lägg till ingrediens" : "Uppdatera ingrediens";
                    OnPropertyChanged(nameof(IsAddingNewIngredient));
                }
            }
        }

        public string AddIngButtonText
        {
            get { return addIngButtonText; }
            set
            {
                if (addIngButtonText != value)
                {
                    addIngButtonText = value;
                    OnPropertyChanged(nameof(AddIngButtonText));
                }
            }
        }

        public void CheckIfDigit_preview(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d$"))
            {
                e.Handled = true;
            }
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (IngredientName.Text == "")
            {
                MessageBox.Show("Välj en ingrediens i listan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (QuantityInput.Text == "")
            {
                MessageBox.Show("Ange mängd för ingrediensen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (UnitComboBox.SelectedItem == null || UnitComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Välj en enhet för ingrediensen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (isAddingNewIngredient && newRecipe.Ingredientlist.Any(x => x.Good.Name == IngredientName.Text))
            {
                MessageBox.Show("Ingrediensen är redan tillagd i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (UnitComboBox.SelectedItem is ComboBoxItem item)
            {
                if (isAddingNewIngredient)
                {
                    string text = item.Content.ToString();
                    NewRecipe.Ingredientlist.Add(new Ingredient(int.Parse(QuantityInput.Text), text, mainWindow.SelectedGood));
                    ResetInput();
                } 
                else
                {
                    var ingredient = NewRecipe.Ingredientlist.FirstOrDefault(x => x.Good.Name == IngredientName.Text);
                    if (ingredient != null)
                    {
                        ingredient.Quantity = int.Parse(QuantityInput.Text);
                        ingredient.Unit = item.Content.ToString();
                        ResetInput();
                    }
                }

            }
        }

        private void AbortNewRecipe_Click(object sender, RoutedEventArgs e)
        {
            IngredientName.Text = "";
            ResetInput();
            BindadBild.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            NewRecipe = new Recipe("");
            mainWindow.SelectedGood = null;
            mainWindow.IngredientPageInstance.SelectedBorder.BorderBrush = Brushes.Transparent;
            mainWindow.IngredientPageInstance.SelectedBorder = null;
        }

        public void ResetInput()
        {
            QuantityInput.Text = "";
            UnitComboBox.SelectedIndex = 0;
        }

        private void RemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Ingredient ingredient)
            {
                NewRecipe.Ingredientlist.Remove(ingredient);
            }
        }

        private void SelectIngredient_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Ingredient ing)
            {
                IsAddingNewIngredient = false;
                mainWindow.SelectedGood = ing.Good;
                mainWindow.IngredientPageInstance.MoveBorder(mainWindow.SelectedGood);
                QuantityInput.Text = ing.Quantity.ToString();
                UnitComboBox.SelectedItem = ing.Unit;
            }
        }

        private async void AddNewRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeName == "")
            {
                MessageBox.Show("Ange ett namn för receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NewRecipe.Ingredientlist.Count < 2)
            {
                MessageBox.Show("Lägg till minst två ingredienser i receptet.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NewRecipe.Name = RecipeName;
            if (await DBHelper.AddRecipeToDB(NewRecipe))
            {
                if (hasAddedImage) KopieraBild();
                IngredientName.Text = "";
                ResetInput();
                BindadBild.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
                NewRecipe = new Recipe("");
                RecipeName = "";
                mainWindow.SelectedGood = null;
            }
            else
            {
                MessageBox.Show("Något gick fel vid tillägget av receptet i databasen.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool skaKopieraBild { get; set; } //Styr om bilden ska kopieras eller inte.
        private bool hasAddedImage { get; set; }
        private bool hasExtension { get; set; }
        private BitmapImage tempBild { get; set; } = new BitmapImage();


        public void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
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
                        BindadBild.Source = bitmapImage;
                        hasAddedImage = true;
                        hasExtension = false;
                        skaKopieraBild = true;
                        AddImagePathToGood();
                    }
                }
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
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
                    NewRecipe.Imagepath = filgenväg;
                    tempBild = img;
                    BindadBild.Source = tempBild;
                    hasAddedImage = true;
                    hasExtension = true;
                    AddImagePathToGood();
                }
            }
        }

        private void AddImagePathToGood()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = _folderpath + @"\Bilder\";
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            NewRecipe.Imagepath = filePath;
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
                    File.WriteAllBytes(NewRecipe.Imagepath, memoryStream.ToArray());
                }
            }

        }

        #endregion

    }
}
