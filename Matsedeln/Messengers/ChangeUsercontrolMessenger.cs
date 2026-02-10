namespace Matsedeln.Messengers
{
    public enum UserControlType { Goods, Recipe, Shopping, Menu }

    public class ChangeUsercontrolMessenger
    {
        public UserControlType Control { get; set; }
        public ChangeUsercontrolMessenger(UserControlType control)
        {
            Control = control;
        }
    }
}
