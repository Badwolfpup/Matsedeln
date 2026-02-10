using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Converters;
using Matsedeln.Messengers;
using Matsedeln.Usercontrols;
using Matsedeln.Utils;
using Matsedeln.Wrappers;
using MatsedelnShared.Models;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Matsedeln.ViewModel
{
    public partial class NewRecipeControlViewModel : ObservableObject, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [ObservableProperty]
        private bool isAddingNewIngredient = true;
        [ObservableProperty]
        private string goodOrRecipeButton = "Visa recept";
        [ObservableProperty]
        private string recipeName = "";
        [ObservableProperty]
        private bool showRest;
        [ObservableProperty]
        private RecipeWrapper newRecipe;
        [ObservableProperty]
        private Ingredient newIngredient;
        [ObservableProperty]
        private string quantity;
        [ObservableProperty]
        private string addRecipeBtnText = "Lägg till recept";
        [ObservableProperty]
        private bool isGoodMode = true;
        [ObservableProperty]
        private Recipe? selectedChildRecipe;
        [ObservableProperty]
        private string childRecipePortions = "1";
        [ObservableProperty]
        private string addItemButtonText = "Lägg till ingrediens";
        public bool IsRecipeMode => !IsGoodMode;
        private bool isAddingNewRecipe = true;
        private bool isEditingRecipe = false;
        private bool showRecipes = true;

        public ImageHandler ImageHandler { get; } = new ImageHandler();

        public NewRecipeControlViewModel()
        {
            NewRecipe = new RecipeWrapper();
            NewIngredient = new Ingredient();
            ImageHandler.SetImage(NewRecipe.ImagePath);
            ImageHandler.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ImageHandler.HasAddedImage))
                    ShowRest = !string.IsNullOrEmpty(RecipeName) && ImageHandler.HasAddedImage;
            };
            WeakReferenceMessenger.Default.Register<PasteImageMessage>(this, (r, m) =>
            {
                ImageHandler.HandlePaste();
                NewRecipe.ImagePath = ImageHandler.ImagePath;
            });
            WeakReferenceMessenger.Default.Register<SelectedGoodsMessenger>(this, (r, m) => LoadIngredient(m.selectedGood.Good));
            WeakReferenceMessenger.Default.Register<SelectedRecipeMessenger>(this, (r, m) =>
            {
                SelectedChildRecipe = m.wrapper.Recipe;
                IsGoodMode = false;
                IsAddingNewIngredient = true;
                ChildRecipePortions = "1";
            });
            WeakReferenceMessenger.Default.Register<CreatingRecipeMessage>(this, (r, m) => m.Reply(ShowRest));
            WeakReferenceMessenger.Default.Register<AddChildRecipeMessenger>(this, (r, m) =>
            {
                if (!NewRecipe.ChildRecipes.Any(x => x.ChildRecipeId == m.recipe.Id) && NewRecipe.Recipe != m.recipe) NewRecipe.ChildRecipes.Add(new RecipeHierarchy(NewRecipe.Recipe, m.recipe));
            });
            WeakReferenceMessenger.Default.Register<EditRecipeMessenger>(this, (r, m) =>
            {
                isAddingNewRecipe = false;
                isEditingRecipe = true;
                NewRecipe = m.wrapper;
                RecipeName = NewRecipe.Name;
                AddRecipeBtnText = "Uppdatera recept";
                var converter = new ImageUrlConverter();
                var convertedUrl = converter.Convert(
                    NewRecipe.ImagePath,
                    typeof(string),
                    null,
                    CultureInfo.CurrentCulture
                ) as string;

                if (!string.IsNullOrEmpty(convertedUrl))
                {
                    ImageHandler.SetImage(convertedUrl);
                }
                else
                {
                    ImageHandler.SetImage("pack://application:,,,/Images/dummybild.png");
                }
                ImageHandler.HasAddedImage = true;
                ShowRest = true;
            });
        }

        private void LoadIngredient(Goods good)
        {
            NewIngredient.Good = good;
            NewIngredient.Initialize();
            IsGoodMode = true;
            IsAddingNewIngredient = true;
        }

        [RelayCommand]
        private void AddIngredient()
        {
            if (IsGoodMode)
            {
                if (NewIngredient.Good == null)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Välj en ingrediens i listan.", isError: true));
                    return;
                }
                if (string.IsNullOrEmpty(Quantity) || NewIngredient.Quantity <= 0)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Ange mängd för ingrediensen.", isError: true));
                    return;
                }
                if (IsAddingNewIngredient && NewRecipe.Ingredientlist.Any(x => x.Good.Name == NewIngredient.Good.Name))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Ingrediensen är redan tillagd i receptet.", isError: true));
                    return;
                }
                NewRecipe.Ingredientlist.Add(NewIngredient);
                WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(NewIngredient.Good.Id));
                NewIngredient = new Ingredient();
                ResetInput();
            }
            else
            {
                if (SelectedChildRecipe == null)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Välj ett recept i listan.", isError: true));
                    return;
                }
                if (!int.TryParse(ChildRecipePortions, out int portions) || portions < 1)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Ange ett giltigt antal (minst 1).", isError: true));
                    return;
                }
                if (NewRecipe.ChildRecipes.Any(x => x.ChildRecipeId == SelectedChildRecipe.Id))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Receptet är redan tillagt.", isError: true));
                    return;
                }
                if (NewRecipe.Recipe == SelectedChildRecipe)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Ett recept kan inte innehålla sig själv.", isError: true));
                    return;
                }
                NewRecipe.ChildRecipes.Add(new RecipeHierarchy(NewRecipe.Recipe, SelectedChildRecipe, portions));
                WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(SelectedChildRecipe.Id));
                SelectedChildRecipe = null;
                ChildRecipePortions = "1";
            }
        }

        [RelayCommand]
        private void ChangePage(Recipe recipe)
        {
            if (showRecipes)
            {
                GoodOrRecipeButton = "Visa ingredienser";
                WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Recipe));
                if (NewRecipe.ChildRecipes.Count > 0)
                {
                    NewRecipe.ChildRecipes.ToList().ForEach(x => WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(x.ChildRecipe.Id)));
                }
                showRecipes = !showRecipes;
            }
            else
            {
                GoodOrRecipeButton = "Visa recept";
                WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Goods));
                if (NewRecipe.Ingredientlist.Count > 0)
                {
                    NewRecipe.Ingredientlist.ToList().ForEach(x => WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(x.Good.Id)));
                }
                showRecipes = !showRecipes;
            }
        }

        [RelayCommand]
        private async Task AddRecipe()
        {
            if (string.IsNullOrEmpty(NewRecipe.Name))
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Ange ett namn för receptet.", isError: true));
                return;
            }
            if (!isEditingRecipe)
            {
                var message = new NameExistsMessenger(NewRecipe.Name);
                WeakReferenceMessenger.Default.Send(message);
                if (message.HasReceivedResponse && message.Response == true)
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Det finns redan ett recept med det namnet", isError: true));
                    return;
                }
            }
            if ((NewRecipe.Ingredientlist.Count + NewRecipe.ChildRecipes.Count) < 2 && NewRecipe.Name != "Färdigmat" && NewRecipe.Name != "Restaurang")
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Lägg till minst en sak i receptet.", isError: true));
                return;
            }
            var api = ApiService.Instance;
            if (ImageHandler.ShouldCopyImage)
            {
                var serverPath = await api.UploadImageAsync(ImageHandler.ImagePath);
                if (string.IsNullOrEmpty(serverPath))
                {
                    WeakReferenceMessenger.Default.Send(new ToastMessage("Något gick fel vid uppladdningen av bilden.", isError: true));
                    return;
                }
                NewRecipe.ImagePath = serverPath;
            }
            var result = await api.PostAsync<Recipe>("api/recipe", NewRecipe.Recipe);
            if (result)
            {
                WeakReferenceMessenger.Default.Send(new RefreshListMessenger());
                WeakReferenceMessenger.Default.Send(new ToastMessage("Receptet har lagts till/uppdaterats."));
                WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(0));

                ResetUserControl();
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ToastMessage("Något gick fel vid tillägget/uppdateringen av receptet.", isError: true));
                return;
            }
        }

        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient == null) return;
            WeakReferenceMessenger.Default.Send(new SetSelectedMessenger(ingredient.Good.Id));
            NewRecipe.Ingredientlist.Remove(ingredient);
        }

        [RelayCommand]
        private void RemoveRecipe(RecipeHierarchy recipe) => NewRecipe.ChildRecipes.Remove(recipe);

        [RelayCommand]
        private void SelectIngredient(Ingredient ingredient)
        {
            IsAddingNewIngredient = false;
            NewIngredient = ingredient;
            Quantity = ingredient.Quantity.ToString();
        }

        [RelayCommand]
        private void ResetUserControl()
        {
            NewRecipe = new RecipeWrapper();
            ImageHandler.Reset(NewRecipe.ImagePath);
            NewIngredient = new Ingredient();
            ResetInput();
            WeakReferenceMessenger.Default.Send(new ClearSelectedMessenger(0));
        }

        public void ResetInput()
        {
            if (isEditingRecipe && NewRecipe.IsDish)
            {
                WeakReferenceMessenger.Default.Send(new ChangePageMessenger(PageType.Dishes));
                WeakReferenceMessenger.Default.Send(new ChangeUsercontrolMessenger(UserControlType.Shopping));
            }
            Quantity = "";
            IsAddingNewIngredient = true;
            IsGoodMode = true;
            SelectedChildRecipe = null;
            ChildRecipePortions = "1";
        }

        partial void OnIsGoodModeChanged(bool value)
        {
            OnPropertyChanged(nameof(IsRecipeMode));
            AddItemButtonText = value ? "Lägg till ingrediens" : "Lägg till recept";
        }

        partial void OnRecipeNameChanged(string value)
        {
            ShowRest = !string.IsNullOrEmpty(value) && ImageHandler.HasAddedImage;
            NewRecipe.Name = RecipeName;
        }

        partial void OnQuantityChanged(string value)
        {
            var converter = new ToSweDecimalConverter();
            var parsed = converter.ConvertBack(value, typeof(double), null, CultureInfo.CurrentCulture);
            if (parsed != DependencyProperty.UnsetValue)
            {
                NewIngredient.Quantity = (double)parsed;
                NewIngredient.GetQuantityInGram(NewIngredient.Quantity);
                NewIngredient.ConvertToOtherUnits();
            }
            else
            {
                NewIngredient.Quantity = 0;
            }
        }
    }
}
