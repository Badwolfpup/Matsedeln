using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Matsedeln.Messengers
{
    public class NameExistsMessenger : RequestMessage<bool>
    {
        public string Name { get; set; }

        public NameExistsMessenger(string name)
        {
            Name = name;
        }
    }
}
