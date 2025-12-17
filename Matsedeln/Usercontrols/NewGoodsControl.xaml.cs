using Matsedeln;
using Matsedeln.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
using Matsedeln.Utils;

namespace Matsedeln.Usercontrols
{
    /// <summary>
    /// Interaction logic for NewGoodsControl.xaml
    /// </summary>
    public partial class NewGoodsControl : UserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly MainWindow mainWindow;
        public NewGoodsControl(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            NyVara = new Goods();
            BindadBild.Source = NyVara.Imagepath != null ? new BitmapImage(new Uri(NyVara.Imagepath)) : new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            IsNewGood = true;
        }

        public NewGoodsControl(MainWindow main, Goods good)
        {
            InitializeComponent();
            mainWindow = main;
            NyVara = good;
            BindadBild.Source = NyVara.Imagepath != null ? new BitmapImage(new Uri(NyVara.Imagepath)) : null;
            NameInput.Text = NyVara.Name;
            if (NyVara.GperDL > 0)
            {
                CboxGperDL.IsChecked = true;
                GperDLInput.Text = NyVara.GperDL.ToString();
            }
            if (NyVara.GperST > 0)
            {
                CboxGperST.IsChecked = true;
                GperSTInput.Text = NyVara.GperST.ToString();
            }
        }

        private Goods NyVara;
        private bool IsNewGood = false;
        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool skaKopieraBild { get; set; } //Styr om bilden ska kopieras eller inte.
        private bool hasAddedImage { get; set; }
        private bool hasExtension { get; set; }
        private BitmapImage tempBild { get; set; } = new BitmapImage();


        public void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsNewGood) return;
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
            if (!IsNewGood) return;
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
                    NyVara.Imagepath = filgenväg;
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

            NyVara.Imagepath = filePath;
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
                    File.WriteAllBytes(NyVara.Imagepath, memoryStream.ToArray());
                }
            }

        }

        #endregion

        private void AbortAddGoods_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            NameInput.Text = "";
            GperDLInput.Text = "";
            GperSTInput.Text = "";
            CboxGperDL.IsChecked = false;
            CboxGperST.IsChecked = false;
            BindadBild.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            if (mainWindow.IngredientPageInstance.SelectedBorder != null)
            {
                mainWindow.IngredientPageInstance.SelectedBorder.BorderBrush = Brushes.Transparent;
                mainWindow.IngredientPageInstance.SelectedBorder = null;
            }
            mainWindow.SelectedGood = null;
        }

        public void CheckIfDigit_preview(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d$"))
            {
                e.Handled = true;
            }
        }

        private async void AddGoodsToDB_Click(object sender, RoutedEventArgs e)
        {
            if (NameInput.Text == "")
            {
                MessageBox.Show("Var god ange ett namn för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CboxGperDL.IsChecked == true && GperDLInput.Text == "")
            {
                MessageBox.Show("Var god ange gram per deciliter för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CboxGperST.IsChecked == true && GperSTInput.Text == "")
            {
                MessageBox.Show("Var god ange gram per styck för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NyVara.Name = NameInput.Text;
            NyVara.GperDL = CboxGperDL.IsChecked == true ? int.Parse(GperDLInput.Text) : 0;
            NyVara.GperST = CboxGperST.IsChecked == true ? int.Parse(GperSTInput.Text) : 0;
            if (IsNewGood)
            {
                var result = await DBHelper.AddGoodsToDB(NyVara);
                if (result && hasAddedImage)
                {
                    KopieraBild();
                }
                ResetForm();
            }
            else
            {
                var result = await DBHelper.UpdateGoodsInDB(NyVara);
                if (result) MessageBox.Show("Vara uppdaterad!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    MessageBox.Show("Något gick fel vid uppdateringen av varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }

        }
    }
}
