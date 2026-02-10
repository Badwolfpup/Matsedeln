namespace Matsedeln.Messengers
{
    public enum PageType { Goods, Recipe, Menu, Dishes }

    public class ChangePageMessenger
    {
        public PageType Page { get; set; }

        public ChangePageMessenger(PageType page)
        {
            Page = page;
        }
    }
}
