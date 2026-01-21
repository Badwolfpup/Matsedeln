using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class SetSelectedMessenger
    {
        public int Id { get; set; }
        public SetSelectedMessenger(int id)
        {
            Id = id;
        }
    }
}
