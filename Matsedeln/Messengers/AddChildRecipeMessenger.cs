using MatsedelnShared.Models;

namespace Matsedeln.Messengers
{
    public class AddChildRecipeMessenger
    {
        public Recipe recipe { get; set; }
        public AddChildRecipeMessenger(Recipe recipe)
        {
            this.recipe = recipe;
        }
    }
}
