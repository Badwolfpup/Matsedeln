using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using MatsedelnShared.Models;

namespace Matsedeln.ViewModel
{
    public partial class RecipePageViewModel : RecipeListViewModelBase
    {
        protected override bool RecipeFilter(Recipe r) => !r.IsDish;

        [RelayCommand]
        public void ShowDishesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Shopping));
        }
    }
}
