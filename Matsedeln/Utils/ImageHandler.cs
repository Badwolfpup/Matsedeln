using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Matsedeln.Utils
{
    public partial class ImageHandler : ObservableObject
    {
        [ObservableProperty]
        private ImageSource image;

        [ObservableProperty]
        private bool hasAddedImage;

        [ObservableProperty]
        private bool shouldCopyImage;

        public string ImagePath { get; set; }

        private string fileextension;
        private bool hasExtension;
        private BitmapImage tempBild = new BitmapImage();

        public void SetImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "pack://application:,,,/Images/dummybild.png";
            }

            Uri finalUri;

            if (path.StartsWith("pack://"))
            {
                finalUri = new Uri(path, UriKind.Absolute);
            }
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
            else
            {
                finalUri = new Uri(path, UriKind.RelativeOrAbsolute);
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = finalUri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                Image = bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Image = new BitmapImage(new Uri("pack://application:,,,/Images/dummybild.png"));
            }
        }

        public void HandlePaste()
        {
            if (Clipboard.ContainsImage())
            {
                BitmapImage bitmapImage = new BitmapImage();

                BitmapSource imageSource = Clipboard.GetImage();
                if (imageSource != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imageSource));
                        encoder.Save(memoryStream);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        tempBild = bitmapImage;
                        Image = bitmapImage;
                        HasAddedImage = true;
                        ShouldCopyImage = true;
                        hasExtension = false;
                        SaveToLocalPath();
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
                    Image = img;
                    string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bilder");
                    ShouldCopyImage = open.FileName.StartsWith(basePath) ? false : true;
                    HasAddedImage = true;
                    hasExtension = true;
                    if (ShouldCopyImage) SaveToLocalPath();
                    else ImagePath = filgenväg;
                }
            }
        }

        private void SaveToLocalPath()
        {
            string _folderpath = AppDomain.CurrentDomain.BaseDirectory;
            string bildfolder = System.IO.Path.Combine(_folderpath, "Bilder");
            if (!Directory.Exists(bildfolder)) Directory.CreateDirectory(bildfolder);

            string filePath = System.IO.Path.Combine(bildfolder, Guid.NewGuid().ToString() + (hasExtension ? fileextension : ".png"));

            ImagePath = filePath;
            SaveToDisk();
        }

        public void SaveToDisk()
        {
            if (!HasAddedImage) return;
            if (tempBild != null)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(tempBild));
                    encoder.Save(memoryStream);

                    File.WriteAllBytes(ImagePath, memoryStream.ToArray());
                }
            }
        }

        public void Reset(string defaultImagePath)
        {
            HasAddedImage = false;
            ShouldCopyImage = false;
            tempBild = new BitmapImage();
            SetImage(defaultImagePath);
        }
    }
}
