using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Messengers;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Matsedeln.ViewModel
{
    public abstract partial class RecipeListViewModelBase : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [ObservableProperty]
        private CollectionViewSource recipesViewSource;

        [ObservableProperty]
        ObservableCollection<RecipeWrapper> recipeList;

        [ObservableProperty]
        private Visibility isLoading = Visibility.Collapsed;

        [ObservableProperty]
        private RecipeWrapper selectedRecipe;

        [ObservableProperty]
        private string filterText = string.Empty;

        protected abstract bool RecipeFilter(Recipe r);

        protected RecipeListViewModelBase()
        {
            WeakReferenceMessenger.Default.Register<NameExistsMessenger>(this, async (r, m) => { if (RecipeList != null && RecipeList.Count > 0) m.Reply(RecipeList.Any(x => x.Name == m.Name)); });
        }

        [RelayCommand]
        private async Task LoadRecipe()
        {
            IsLoading = Visibility.Visible;

            try
            {
                var api = ApiService.Instance;
                var list = await api.GetListAsync<Recipe>("api/recipe");

                RecipeList = new ObservableCollection<RecipeWrapper>(
                    list?.Where(y => RecipeFilter(y)).Select(x => new RecipeWrapper(x)) ?? Enumerable.Empty<RecipeWrapper>()
                );

                if (RecipesViewSource != null)
                {
                    RecipesViewSource.Filter -= FilterRecipe;
                }

                RecipesViewSource = new CollectionViewSource { Source = RecipeList };
                RecipesViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                RecipesViewSource.Filter += FilterRecipe;
                RecipesViewSource.View.Culture = new CultureInfo("sv-SE");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                RecipeList = new ObservableCollection<RecipeWrapper>();
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }
        }

        private void FilterRecipe(object sender, FilterEventArgs e)
        {
            if (e.Item is RecipeWrapper wrapper)
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = wrapper.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 || wrapper.IsSelected;
                }
            }
        }

        [RelayCommand]
        private void EditRecipe(RecipeWrapper wrapper)
        {
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Recipe));
            WeakReferenceMessenger.Default.Send(new EditRecipeMessenger(wrapper));
        }

        [RelayCommand]
        private async Task DeleteRecipe()
        {
            var sum = RecipeList.Sum(x => x.IsHighlighted ? 1 : 0);
            if (sum == 0)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Du måste välja något innan du kan radera.", isError: true));
                return;
            }
            else if (sum > 1)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Du får bara välja ett recept innan du kan radera.", isError: true));
                return;
            }
            var SelectedRecipe = RecipeList.FirstOrDefault(x => x.IsHighlighted);
            if (SelectedRecipe == null)
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Det gick inte att radera. Problem att hitta datacontext", isError: true));
                return;
            }
            MessageBoxResult result = MessageBox.Show("Vill du verkligen radera detta recept?", "Confirm delettion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var api = ApiService.Instance;
                if (!await api.DeleteAsync($"api/recipe/{SelectedRecipe.Recipe.Id}"))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Det gick inte att radera receptet", isError: true));
                    return;
                }
                WeakReferenceMessenger.Default.Send(new ToastMessage("Receptet har raderats."));
                RecipeList.Remove(SelectedRecipe);
                RecipesViewSource.View.Refresh();
            }
        }

        [RelayCommand]
        public void ShowMenuPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Menu));
            WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Menu));
        }

        [RelayCommand]
        public void ShowIngredientsPage()
        {
            WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Goods));
            var message = new CreatingRecipeMessage();
            WeakReferenceMessenger.Default.Send(message);
            if (message.HasReceivedResponse && message.Response == false) WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Goods));
        }

        [RelayCommand]
        private void SelectBorder(RecipeWrapper wrapper)
        {
            if (wrapper == null) return;
            wrapper.IsHighlighted = !wrapper.IsHighlighted;
            WeakReferenceMessenger.Default.Send(new UpdateShoppingListMessenger(wrapper, wrapper.IsHighlighted));
            WeakReferenceMessenger.Default.Send(new SelectedRecipeMessenger(wrapper));
            WeakReferenceMessenger.Default.Send(new AddUpdateMenu(wrapper));
        }
    }
}
