using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
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
using System.Windows.Shapes;

namespace Matsedeln.ViewModel
{
    public partial class NewGoodsControlViewModel: ObservableObject
    {
   

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
        [ObservableProperty]
        private bool _shouldCopyImage;
        private GoodsWrapper goodsWrapper;

        public NewGoodsControlViewModel()
        {
            NewGood = new Goods();
            GoodsImage = new BitmapImage(new Uri(NewGood.ImagePath));
            WeakReferenceMessenger.Default.Register<SelectedGoodsMessenger>(this, (r, m) => { 
                NewGood = m.selectedGood.good; 
                goodsWrapper = m.selectedGood; 
                ButtonText = "Uppdatera vara"; 
                SelectedGood(); 
            });
            WeakReferenceMessenger.Default.Register<AppData.PasteImageMessage>(this, (r, m) => OnPasteExecuted());
        }

        private void SelectedGood()
        {
            IsGperDLEnabled = NewGood.GramsPerDeciliter > 0;
            IsGperSTEnabled = NewGood.GramsPerStick > 0;
            GperDL = NewGood.GramsPerDeciliter > 0 ? NewGood.GramsPerDeciliter.ToString() : string.Empty;
            GperST = NewGood.GramsPerStick > 0 ? NewGood.GramsPerStick.ToString() : string.Empty;
            SetImage(NewGood.ImagePath);
        }


        [RelayCommand]
        private void ShowAddRecipeUserControl()
        {
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger("recipe"));
            if (NewGood != null) WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(NewGood.Id));
        }

        [RelayCommand]
        private void ResetUserControl()
        {
            NewGood = new Goods();
            SelectedGood();
            if (goodsWrapper != null) goodsWrapper.IsHighlighted = false;
            ButtonText = "Lägg till vara";
        }

        partial void OnGperDLChanged(string value)
        {
            NewGood.GramsPerDeciliter = int.TryParse(value, out int result) ? result : 0;
        }
        partial void OnGperSTChanged(string value)
        {
            NewGood.GramsPerStick = int.TryParse(value, out int result) ? result : 0;
        }

        partial void OnIsGperDLEnabledChanged(bool value)
        {
            if (!value) GperDL = "0";
        }

        partial void OnIsGperSTEnabledChanged(bool value)
        {
            if (!value) GperST = "0";
        }

        [RelayCommand]
        private async Task AddGoodsToDB()
        {
            if (string.IsNullOrEmpty(NewGood.Name))
            {
                MessageBox.Show("Var god ange ett namn för varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var message = new NameExistsMessenger(NewGood.Name);
            WeakReferenceMessenger.Default.Send(message);
            if (message.HasReceivedResponse && message.Response == true)
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
            var api = new ApiService();
            var serverPath = await api.UploadImageAsync(NewGood.ImagePath);
            if (string.IsNullOrEmpty(serverPath))
            {
                MessageBox.Show("Något gick fel vid uppladdningen av bilden.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NewGood.ImagePath = serverPath;
            var result = await api.PostAsync<Goods>("api/goods", NewGood);
            if (result)
            {
                WeakReferenceMessenger.Default.Send(new RefreshListMessenger());
                WeakReferenceMessenger.Default.Send(new ToastMessage("Varan har lagts till/uppdaterats."));
                ResetUserControl();
            }
            else
            {
                MessageBox.Show("Något gick fel vid tillägget/uppdateringen av varan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        #region Handle Images
        private string fileextension; //Håller koll på filändelsen på bilden.

        private bool hasAddedImage { get; set; }
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
                GoodsImage = bitmap;
            }
            catch (Exception ex)
            {
                // If everything fails, hard-code the fallback
                GoodsImage = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
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
                        GoodsImage = bitmapImage;
                        hasAddedImage = true;
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
                    GoodsImage = img;
                    string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
                    ShouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;
                    hasAddedImage = true;
                    hasExtension = true;
                    if (ShouldCopyImage) AddImagePathToGood();
                    else NewGood.ImagePath = filgenväg;
                }
            }
        }

        private void AddImagePathToGood()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = System.IO.Path.Combine(_folderpath, "Bilder");
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            NewGood.ImagePath = filePath;
            KopieraBild();
        }

        public void KopieraBild()
        {

            if (!hasAddedImage) return;
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
