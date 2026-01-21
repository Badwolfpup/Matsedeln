using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class ChangePageMessenger
    {
        public string TypeOfPage { get; set; }

        public ChangePageMessenger(string typeOfPage)
        {
            TypeOfPage = typeOfPage;
        }
    }
}
