using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using MatsedelnShared.Models;

namespace Matsedeln.ViewModel
{
    public partial class DishesPageViewModel : RecipeListViewModelBase
    {
        protected override bool RecipeFilter(Recipe r) => r.IsDish;

        [RelayCommand]
        public void ShowRecipesPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Recipe));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Recipe));
        }
    }
}
