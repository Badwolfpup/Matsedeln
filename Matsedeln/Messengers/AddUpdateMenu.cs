using Matsedeln.Wrappers;

namespace Matsedeln.Messengers
{
    public class AddUpdateMenu
    {
        public RecipeWrapper wrapper { get; set; }


        public AddUpdateMenu(RecipeWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }
}
