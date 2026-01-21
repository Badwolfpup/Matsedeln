using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class SelectedRecipeMessenger
    {
        public Recipe recipe { get; set; }

        public SelectedRecipeMessenger(Recipe recipe)
        {
            this.recipe = recipe;
        }
    }
}
