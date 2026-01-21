using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
