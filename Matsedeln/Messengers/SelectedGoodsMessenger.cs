using Matsedeln.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class SelectedGoodsMessenger
    {
        public GoodsWrapper selectedGood { get; set; }

        public SelectedGoodsMessenger(GoodsWrapper selectedGood)
        {
            this.selectedGood = selectedGood;
        }
    }
}
