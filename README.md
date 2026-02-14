# Matsedeln

A meal planning and recipe management system with a WPF desktop client, ASP.NET Core API server, and shared model library. "Matsedeln" means "The Menu" in Swedish.

## Architecture

The solution consists of three projects:

### Matsedeln (WPF Desktop Client)
The main user interface built with WPF and the MVVM pattern.

- 8+ ViewModels with base class inheritance
- 11 value converters for data binding
- 15 messenger classes for inter-component communication
- Pages for dishes, ingredients, menus, and recipes
- User controls for weekly menus, shopping lists, recipe creation, and more

### Matserver (ASP.NET Core API)
REST API backend handling data persistence and image management.

- REST controllers for Goods, Images, MenuEntries, Recipes, and RecipeHierarchies
- Entity Framework Core with code-first migrations
- Image upload and serving

### MatsedelnShared (Shared Library)
Shared models and database context used by both client and server.

- Models: Goods, Ingredient, MenuEntry, Recipe, RecipeHierarchy
- Shared ApplicationDbContext

## Technologies

- **Desktop:** C#, WPF (XAML), MVVM pattern
- **Backend:** ASP.NET Core, Entity Framework Core
- **Shared:** .NET class library
- **Database:** SQL Server / SQLite with EF Core migrations
- **Communication:** REST API (HttpClient)

## Features

- **Recipe Management** - Create, edit, and organize recipes with images
- **Recipe Hierarchies** - Support for sub-recipes (e.g., a sauce within a main dish)
- **Ingredient Tracking** - Manage ingredients with quantities and units
- **Weekly Menu Planning** - Plan meals for the week with drag-and-drop
- **Shopping List Generation** - Auto-generate shopping lists from planned menus
- **Image Support** - Upload and display recipe images
- **Toast Notifications** - User feedback for actions

## Project Structure

```
Matsedeln/
├── Matsedeln/                # WPF Desktop Client
│   ├── Pages/                # Main application pages
│   ├── ViewModel/            # MVVM ViewModels
│   ├── Converters/           # 11 value converters
│   ├── Messengers/           # 15 messenger classes
│   ├── Usercontrols/         # Reusable UI components
│   ├── Utils/                # API service, image handler
│   └── Wrappers/             # Observable wrappers
├── Matserver/                # ASP.NET Core API
│   ├── Controllers/          # REST controllers
│   ├── Migrations/           # EF Core migrations
│   └── wwwroot/images/       # Uploaded recipe images
└── MatsedelnShared/          # Shared library
    └── Models/               # Shared entity models
```

## Getting Started

### Server
```bash
cd Matserver
dotnet restore
dotnet ef database update
dotnet run
```

### Desktop Client
Open `Matsedeln.slnx` in Visual Studio and run the Matsedeln project. Ensure the server is running first.
