using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class ChangeUsercontrolMessenger
    {
        public string TypeOfControl { get; set; }
        public ChangeUsercontrolMessenger(string typeOfControl)
        {
            TypeOfControl = typeOfControl;
        }
    }
}
