using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class ClearSelectedMessenger
    {
        public int Id { get; set; }
        public ClearSelectedMessenger(int id)
        {
            Id = id;
        }
    }
}
