using Matsedeln.Wrappers;

namespace Matsedeln.Messengers
{
    public class SelectedRecipeMessenger
    {
        public RecipeWrapper wrapper { get; set; }

        public SelectedRecipeMessenger(RecipeWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }
}
