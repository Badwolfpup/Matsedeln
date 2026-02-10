using Matsedeln.Wrappers;

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
