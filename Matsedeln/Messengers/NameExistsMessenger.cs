using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class NameExistsMessenger: RequestMessage<bool>
    {
        public string Name { get; set; }

        public NameExistsMessenger(string name)
        {
            Name = name;
        }
    }
}
