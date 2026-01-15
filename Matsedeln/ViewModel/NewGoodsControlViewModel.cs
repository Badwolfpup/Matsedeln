using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Models;
using Matsedeln.Usercontrols;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public partial class NewGoodsControlViewModel: ObservableObject
    {
   

        public AppData Ad { get; } = AppData.Instance;
        [ObservableProperty]
        private Goods newGood;
        [ObservableProperty]
        private string buttonText = "Lägg till vara";
        [ObservableProperty]
        private string imagePath;
        [ObservableProperty]
        private ImageSource goodsImage;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string gperDL;  
        [ObservableProperty]
        private string gperST;
        [ObservableProperty]
        private bool isGperDLEnabled = false;
        [ObservableProperty]
        private bool isGperSTEnabled = false;
        private bool IsNewGood => !Ad.GoodsList.Any(g => g.Id == NewGood.Id);
        private bool _shouldCopyImage;

        public NewGoodsControlViewModel()
        {
            NewGood = new Goods();
            GoodsImage = new BitmapImage(new Uri(NewGood.ImagePath));
            WeakReferenceMessenger.Default.Register<AppData.PassGoodsToUCMessage>(this, (r, m) => { NewGood = m.good; UpdateUI(); });
            WeakReferenceMessenger.Default.Register<AppData.PasteImageMessage>(this, (r, m) => OnPasteExecuted());
        }

        private void UpdateUI()
        {
            IsGperDLEnabled = NewGood.GramsPerDeciliter > 0;
            IsGperSTEnabled = NewGood.GramsPerStick > 0;
            GperDL = NewGood.GramsPerDeciliter > 0 ? NewGood.GramsPerDeciliter.ToString() : string.Empty;
            GperST = NewGood.GramsPerStick > 0 ? NewGood.GramsPerStick.ToString() : string.Empty;
            ButtonText = IsNewGood ? "Lägg till vara" : "Uppdatera vara";
            GoodsImage = new BitmapImage(new Uri(NewGood.ImagePath));
        }

        [RelayCommand]
        private void ResetGperDL(bool ischecked)
        {
           if (!ischecked) GperDL = "0";
        }

        [RelayCommand]
        private void ResetGperST(bool ischecked)
        {
            if (!ischecked) GperST = "0";

        }

        [RelayCommand]
        private void ShowAddRecipeUserControl()
        {
            Ad.CurrentUserControl = new NewRecipeControl();
            WeakReferenceMessenger.Default.Send(new AppData.RemoveAllHighlightBorderMessage());
        }

        [RelayCommand]
        private void AbortAddGoods()
        {
            NewGood = new Goods();
            UpdateUI();
            WeakReferenceMessenger.Default.Send(new AppData.ResetBorderMessage());
        }

        partial void OnGperDLChanged(string value)
        {
            NewGood.GramsPerDeciliter = int.TryParse(value, out int result) ? result : 0;
        }

        partial void OnGperSTChanged(string value)
        {
            NewGood.GramsPerStick = int.TryParse(value, out int result) ? result : 0;
        }
        [RelayCommand]
        private async Task AddGoodsToDB()
        {
            if (string.IsNullOrEmpty(NewGood.Name))
            {
                MessageBox.Show("Var god ange ett namn för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Ad.GoodsList.Any(x => x.Name == NewGood.Name && IsNewGood))
            {
                MessageBox.Show("Det finns redan en vara med det namnet");
                return;
            }
            if (IsGperDLEnabled == true && string.IsNullOrEmpty(GperDL))
            {
                MessageBox.Show("Var god ange gram per deciliter för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsGperSTEnabled == true && string.IsNullOrEmpty(GperST))
            {
                MessageBox.Show("Var god ange gram per styck för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsNewGood)
            {
                var result = await Ad.GoodsService.AddGoods(NewGood, Ad.GoodsList);
                if (result && _shouldCopyImage)
                {
                    KopieraBild();
                    GoodsImage = new BitmapImage(new Uri(NewGood.ImagePath));
                }
                NewGood = new Goods();
                UpdateUI();
            }
            else
            {
                var result = await Ad.GoodsService.UpdateGoods(NewGood);
                if (result) MessageBox.Show("Vara uppdaterad!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    MessageBox.Show("Något gick fel vid uppdateringen av varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }

        }

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool hasAddedImage { get; set; }
        private bool hasExtension { get; set; }
        private BitmapImage tempBild { get; set; } = new BitmapImage();


        public void OnPasteExecuted()
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
                        GoodsImage = bitmapImage;
                        hasAddedImage = true;
                        _shouldCopyImage = true;
                        hasExtension = false;
                        AddImagePathToGood();
                    }
                }
            }
        }

        [RelayCommand]
        private void ImageMouseDown()
        {
            if (!IsNewGood) return;
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
                    tempBild = img;
                    GoodsImage = img;
                    string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
                    _shouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;
                    hasAddedImage = true;
                    hasExtension = true;
                    if (_shouldCopyImage) AddImagePathToGood();
                    else NewGood.ImagePath = filgenväg;
                }
            }
        }

        private void AddImagePathToGood()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = Path.Combine(_folderpath, "Bilder");
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            NewGood.ImagePath = filePath;
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
                    File.WriteAllBytes(NewGood.ImagePath, memoryStream.ToArray());
                }
            }

        }

        #endregion
    }


}
