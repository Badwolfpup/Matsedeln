using Matsedeln.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Matsedeln.Utils
{
    public class AppDbContext: DbContext
    {
        //private readonly string connectionString = Matsedeln.App.Configuration.GetConnectionString("MatsedelnDB");

        string connectionString = "Server=localhost;Database=MatsedelnDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"; 
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<MenuEntry> MenuItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.ChildRecipes)
                .WithOne(r => r.ParentRecipe)
                .HasForeignKey(r => r.ParentRecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredientlist)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ingredient>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Quantity)
                .HasDefaultValue(0);

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Unit)
                .HasDefaultValue("g");

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Good)
                .WithMany()
                .HasForeignKey(i => i.GoodsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany()
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goods>()
                .Property(g => g.GramsPerDeciliter)
                .HasDefaultValue(0);

            modelBuilder.Entity<Goods>()
                .Property(g => g.GramsPerStick)
                .HasDefaultValue(0);

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
}
