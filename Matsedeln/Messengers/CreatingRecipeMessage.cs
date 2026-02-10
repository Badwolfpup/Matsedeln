using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Matsedeln.Messengers
{
    public class CreatingRecipeMessage : RequestMessage<bool>
    {
        public CreatingRecipeMessage()
        {
        }
    }
}
