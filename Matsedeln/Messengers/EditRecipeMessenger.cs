using Matsedeln.Wrappers;

namespace Matsedeln.Messengers
{
    public class EditRecipeMessenger
    {
        public RecipeWrapper wrapper { get; set; }

        public EditRecipeMessenger(RecipeWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }
}
