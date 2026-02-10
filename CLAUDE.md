# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Matsedeln is a Swedish recipe and weekly menu planning application. It consists of three projects:

- **Matsedeln** - WPF desktop client (net9.0-windows)
- **MatsedelnShared** - Shared library with EF Core models and DbContext
- **Matserver** - ASP.NET Core Web API backend

## Build and Run Commands

```bash
# Build entire solution
dotnet build Matsedeln.slnx

# Run the API server (required for client)
dotnet run --project Matserver/Matserver.csproj

# Run the WPF client
dotnet run --project Matsedeln/Matsedeln.csproj

# Apply EF Core migrations
dotnet ef database update --project MatsedelnShared --startup-project Matserver

# Add a new migration
dotnet ef migrations add <MigrationName> --project MatsedelnShared --startup-project Matserver
```

## Architecture

### MVVM Pattern with CommunityToolkit.Mvvm
- ViewModels use `[ObservableProperty]` attributes for auto-generated property change notification
- **Wrapper classes** (`GoodsWrapper`, `RecipeWrapper`, `MenuWrapper`) wrap domain models to add UI state (`IsSelected`, `IsHighlighted`) without polluting models

### Messenger Pattern for Communication
Uses `WeakReferenceMessenger` for loose coupling between components:
- Page/control navigation: `ChangePageMessenger`, `ChangeUsercontrolMessenger`
- Data selection: `SelectedGoodsMessenger`, `SelectedRecipeMessenger`
- List updates: `RefreshListMessenger`, `UpdateShoppingListMessenger`
- User feedback: `ToastMessage`

### API Communication
`ApiService.cs` provides generic HTTP methods. Base URL is conditional:
- DEBUG: `http://localhost:5127`
- RELEASE: configured domain

API requires `X-Api-Key` header (bypassed in Development environment).

### Database
SQL Server with EF Core. Key relationships:
- Recipe 1:many Ingredient (cascade delete)
- Ingredient many:1 Goods
- Recipe parent-child via RecipeHierarchy
- MenuEntry optional FK to Recipe (lunch/dinner)

## Swedish Localization

The app uses Swedish throughout:
- UI text in Swedish
- Swedish cooking units: g, kg, dl, l, st, msk (tablespoon), tsk (teaspoon), krm (pinch)
- Date formatting with Swedish culture (sv-SE)
- Confirmation dialogs in Swedish

## Key Files

- `Matsedeln/Utils/ApiService.cs` - All API communication
- `Matsedeln/ViewModel/MainViewModel.cs` - Page/control navigation hub
- `MatsedelnShared/ApplicationDbContext.cs` - Database schema definition
- `Matserver/Program.cs` - API startup and middleware configuration
- `Matserver/Controllers/RecipeController.cs` - Recursive recipe preparation logic

## Notes

- Swagger UI available at `/swagger/ui` in Development
- Recipe hierarchy allows recipes to contain other recipes as sub-components
- Ingredient quantities stored in grams with on-the-fly unit conversion
