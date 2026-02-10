using Matsedeln.Wrappers;

namespace Matsedeln.Messengers
{
    public class RemoveRecipeMenuMessenger
    {
        public MenuWrapper menu { get; set; }

        public bool IsLunch { get; set; }
        public RemoveRecipeMenuMessenger(MenuWrapper menu, bool isLunch)
        {
            this.menu = menu;
            IsLunch = isLunch;
        }
    }
}
