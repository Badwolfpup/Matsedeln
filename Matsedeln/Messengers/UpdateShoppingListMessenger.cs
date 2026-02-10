using Matsedeln.Wrappers;

namespace Matsedeln.Messengers
{
    public class UpdateShoppingListMessenger
    {
        public RecipeWrapper wrapper { get; set; }

        public bool AddorRemove { get; set; }

        public UpdateShoppingListMessenger(RecipeWrapper recipeWrapper, bool addOrRemove)
        {
            wrapper = recipeWrapper;
            AddorRemove = addOrRemove;
        }
    }
}
