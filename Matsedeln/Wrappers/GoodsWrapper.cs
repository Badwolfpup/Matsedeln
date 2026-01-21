using CommunityToolkit.Mvvm.ComponentModel;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Wrappers
{
    public partial class GoodsWrapper: ObservableObject
    {
        public Goods good { get; set; } // Reference to actual model
        [ObservableProperty]
        private bool _isHighlighted;
        [ObservableProperty]
        private bool _isSelected;
    }
}
