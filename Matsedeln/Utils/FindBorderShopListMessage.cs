using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows.Controls;

public class FindBorderShopListMessage : RequestMessage<IEnumerable<Border>>
{
    public ItemsControl ItemsControl { get; }

    public FindBorderShopListMessage(ItemsControl itemsControl)
    {
        ItemsControl = itemsControl;
    }
}
