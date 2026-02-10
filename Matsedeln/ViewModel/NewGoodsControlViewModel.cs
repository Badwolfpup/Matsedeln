using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;

namespace Matsedeln.ViewModel
{
    public partial class NewGoodsControlViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [ObservableProperty]
        private string title = "Lägg till vara";
        [ObservableProperty]
        private Goods newGood;
        [ObservableProperty]
        private string buttonText = "Lägg till vara";
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
        private GoodsWrapper goodsWrapper;
        private bool isNewGood = true;

        public ImageHandler ImageHandler { get; } = new ImageHandler();

        public NewGoodsControlViewModel()
        {
            NewGood = new Goods();
            ImageHandler.SetImage(NewGood.ImagePath);
            WeakReferenceMessenger.Default.Register<SelectedGoodsMessenger>(this, (r, m) =>
            {
                if (m.selectedGood == null)
                {
                    ResetUserControl();
                    return;
                }
                NewGood = m.selectedGood.Good;
                goodsWrapper = m.selectedGood;
                ButtonText = "Uppdatera vara";
                Title = "Uppdatera vara";
                isNewGood = false;
                SelectedGood();
            });
            WeakReferenceMessenger.Default.Register<PasteImageMessage>(this, (r, m) =>
            {
                ImageHandler.HandlePaste();
                NewGood.ImagePath = ImageHandler.ImagePath;
            });
        }

        private void SelectedGood()
        {
            IsGperDLEnabled = NewGood.GramsPerDeciliter > 0;
            IsGperSTEnabled = NewGood.GramsPerStick > 0;
            GperDL = NewGood.GramsPerDeciliter > 0 ? NewGood.GramsPerDeciliter.ToString() : string.Empty;
            GperST = NewGood.GramsPerStick > 0 ? NewGood.GramsPerStick.ToString() : string.Empty;
            ImageHandler.SetImage(NewGood.ImagePath);
        }


        [RelayCommand]
        private void ResetUserControl()
        {
            NewGood = new Goods();
            SelectedGood();
            Title = "Lägg till vara";
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
                WeakReferenceMessenger.Default.Send(new ToastMessage("Var god ange ett namn för varan.", isError: true));
                return;
            }
            if (isNewGood)
            {
                var message = new NameExistsMessenger(NewGood.Name);
                WeakReferenceMessenger.Default.Send(message);
                if (message.HasReceivedResponse && message.Response == true)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Det finns redan en vara med det namnet", isError: true));
                    return;
                }
            }
            if (IsGperDLEnabled == true && string.IsNullOrEmpty(GperDL))
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Var god ange gram per deciliter för varan.", isError: true));
                return;
            }
            if (IsGperSTEnabled == true && string.IsNullOrEmpty(GperST))
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Var god ange gram per styck för varan.", isError: true));
                return;
            }
            var api = ApiService.Instance;
            if (isNewGood)
            {
                var serverPath = await api.UploadImageAsync(ImageHandler.ImagePath ?? NewGood.ImagePath);
                if (string.IsNullOrEmpty(serverPath))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Något gick fel vid uppladdningen av bilden.", isError: true));
                    return;
                }
                NewGood.ImagePath = serverPath;
            }
            var result = await api.PostAsync<Goods>("api/goods", NewGood);
            if (result)
            {
                WeakReferenceMessenger.Default.Send(new RefreshListMessenger());
                WeakReferenceMessenger.Default.Send(new ToastMessage("Varan har lagts till/uppdaterats."));
                ResetUserControl();
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Något gick fel vid tillägget/uppdateringen av varan.", isError: true));
                return;
            }
        }
    }
}
