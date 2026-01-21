using MatsedelnShared.Models;
using Microsoft.EntityFrameworkCore;

namespace MatsedelnShared;

public class ApplicationDbContext : DbContext
{
    // The constructor must accept options to allow the Server to pass the connection string
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // List your database tables here
    public DbSet<Goods> Goods { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<Recipe> Recipes { get; set; }

    public DbSet<RecipeHierarchy> RecipeHierarchies { get; set; }

    public DbSet<MenuEntry> MenuItems { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{

    //    modelBuilder.Entity<Recipe>()
    //        .HasMany(r => r.Ingredientlist)
    //        .WithOne(i => i.Recipe)
    //        .HasForeignKey(i => i.RecipeId)
    //        .OnDelete(DeleteBehavior.Cascade);

    //    modelBuilder.Entity<RecipeHierarchy>()
    //        .HasKey(rh => rh.Id);

    //    modelBuilder.Entity<RecipeHierarchy>()
    //        .HasOne(rh => rh.ParentRecipe)
    //        .WithMany(r => r.ChildRecipes)
    //        .HasForeignKey(rh => rh.ParentRecipeId)
    //        .OnDelete(DeleteBehavior.NoAction);

    //    modelBuilder.Entity<RecipeHierarchy>()
    //        .HasOne(rh => rh.ChildRecipe)
    //        .WithMany(r => r.ParentRecipes)
    //        .HasForeignKey(rh => rh.ChildRecipeId)
    //        .OnDelete(DeleteBehavior.NoAction);

    //    modelBuilder.Entity<Ingredient>()
    //        .HasKey(i => i.Id);

    //    modelBuilder.Entity<Ingredient>()
    //        .Property(i => i.Quantity)
    //        .HasDefaultValue(0);

    //    modelBuilder.Entity<Ingredient>()
    //        .Property(i => i.Unit)
    //        .HasDefaultValue("g");

    //    modelBuilder.Entity<Ingredient>()
    //        .HasOne(i => i.Good)
    //        .WithMany()
    //        .HasForeignKey(i => i.GoodsId)
    //        .OnDelete(DeleteBehavior.Cascade);

    //    modelBuilder.Entity<Ingredient>()
    //        .HasOne(i => i.Recipe)
    //        .WithMany()
    //        .HasForeignKey(i => i.RecipeId)
    //        .OnDelete(DeleteBehavior.Cascade);

    //    modelBuilder.Entity<Goods>()
    //        .Property(g => g.GramsPerDeciliter)
    //        .HasDefaultValue(0);

    //    modelBuilder.Entity<Goods>()
    //        .Property(g => g.GramsPerStick)
    //        .HasDefaultValue(0);

    //    modelBuilder.Entity<MenuEntry>()
    //        .HasOne(r => r.LunchRecipe)
    //        .WithMany()
    //        .HasForeignKey(r => r.LunchRecipeId)
    //        .OnDelete(DeleteBehavior.NoAction);

    //    modelBuilder.Entity<MenuEntry>()
    //        .HasOne(r => r.DinnerRecipe)
    //        .WithMany()
    //        .HasForeignKey(r => r.DinnerRecipeId)
    //        .OnDelete(DeleteBehavior.NoAction);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 1. RECIPE & INGREDIENT (Combined Fix) ---
        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.Recipe)               // Ingredient has one Recipe
            .WithMany(r => r.Ingredientlist)     // Recipe has many Ingredients
            .HasForeignKey(i => i.RecipeId)      // Use this ID
            .OnDelete(DeleteBehavior.Cascade);   // If Recipe dies, Ingredients die

        // --- 2. RECIPE HIERARCHY (Keep this) ---
        modelBuilder.Entity<RecipeHierarchy>().HasKey(rh => rh.Id);

        modelBuilder.Entity<RecipeHierarchy>()
            .HasOne(rh => rh.ParentRecipe)
            .WithMany(r => r.ChildRecipes)
            .HasForeignKey(rh => rh.ParentRecipeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RecipeHierarchy>()
            .HasOne(rh => rh.ChildRecipe)
            .WithMany(r => r.ParentRecipes)
            .HasForeignKey(rh => rh.ChildRecipeId)
            .OnDelete(DeleteBehavior.NoAction);

        // --- 3. INGREDIENT & GOODS ---
        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.Good)
            .WithMany() // Good doesn't need a list of Ingredients
            .HasForeignKey(i => i.GoodsId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- 4. DEFAULTS ---
        modelBuilder.Entity<Ingredient>().Property(i => i.Quantity).HasDefaultValue(0);
        modelBuilder.Entity<Ingredient>().Property(i => i.Unit).HasDefaultValue("g");
        modelBuilder.Entity<Goods>().Property(g => g.GramsPerDeciliter).HasDefaultValue(0);
        modelBuilder.Entity<Goods>().Property(g => g.GramsPerStick).HasDefaultValue(0);

        // --- 5. MENU ENTRIES ---
        modelBuilder.Entity<MenuEntry>()
            .HasOne(r => r.LunchRecipe)
            .WithMany()
            .HasForeignKey(r => r.LunchRecipeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MenuEntry>()
            .HasOne(r => r.DinnerRecipe)
            .WithMany()
            .HasForeignKey(r => r.DinnerRecipeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
