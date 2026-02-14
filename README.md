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
â”œâ”€â”€ Matsedeln/                # WPF Desktop Client
â”‚   â”œâ”€â”€ Pages/                # Main application pages
â”‚   â”œâ”€â”€ ViewModel/            # MVVM ViewModels
â”‚   â”œâ”€â”€ Converters/           # 11 value converters
â”‚   â”œâ”€â”€ Messengers/           # 15 messenger classes
â”‚   â”œâ”€â”€ Usercontrols/         # Reusable UI components
â”‚   â”œâ”€â”€ Utils/                # API service, image handler
â”‚   â””â”€â”€ Wrappers/             # Observable wrappers
â”œâ”€â”€ Matserver/                # ASP.NET Core API
â”‚   â”œâ”€â”€ Controllers/          # REST controllers
â”‚   â”œâ”€â”€ Migrations/           # EF Core migrations
â”‚   â””â”€â”€ wwwroot/images/       # Uploaded recipe images
â””â”€â”€ MatsedelnShared/          # Shared library
    â””â”€â”€ Models/               # Shared entity models
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
