using CommunityToolkit.Mvvm.ComponentModel;
using MatsedelnShared.Models;

namespace Matsedeln.Wrappers
{
    // Wraps Goods to add UI-specific state (IsHighlighted, IsSelected) and
    // PropertyChanged notification. The Goods model itself is a plain POCO
    // kept in the shared library without UI framework dependencies.
    public partial class GoodsWrapper : ObservableObject
    {
        private readonly Goods _good;

        // Exposes the underlying model for API calls and data operations.
        public Goods Good => _good;

        // Proxy properties that delegate to the model and raise PropertyChanged.
        public int Id
        {
            get => _good.Id;
            set => SetProperty(_good.Id, value, _good, (g, v) => g.Id = v);
        }

        public string Name
        {
            get => _good.Name;
            set => SetProperty(_good.Name, value, _good, (g, v) => g.Name = v);
        }

        public string? ImagePath
        {
            get => _good.ImagePath;
            set => SetProperty(_good.ImagePath, value, _good, (g, v) => g.ImagePath = v);
        }

        public int GramsPerDeciliter
        {
            get => _good.GramsPerDeciliter;
            set => SetProperty(_good.GramsPerDeciliter, value, _good, (g, v) => g.GramsPerDeciliter = v);
        }

        public int GramsPerStick
        {
            get => _good.GramsPerStick;
            set => SetProperty(_good.GramsPerStick, value, _good, (g, v) => g.GramsPerStick = v);
        }

        [ObservableProperty]
        private bool _isHighlighted;
        [ObservableProperty]
        private bool _isSelected;

        public GoodsWrapper(Goods good)
        {
            _good = good;
        }

        public GoodsWrapper() : this(new Goods())
        {
        }
    }
}
