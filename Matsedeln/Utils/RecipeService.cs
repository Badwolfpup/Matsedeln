using Matsedeln.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Matsedeln.Utils
{
    public class RecipeService
    {
        public async Task<ObservableCollection<Recipe>> GetRecipes()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var recipes = new ObservableCollection<Recipe>(
                        await context.Recipes
                            .Include(r => r.Ingredientlist)
                                .ThenInclude(i => i.Good)
                            .Include(r => r.ChildRecipes)  // NEW: Load ChildRecipes
                                .ThenInclude(cr => cr.ChildRecipe)  // Load the actual child Recipe
                                    .ThenInclude(child => child.Ingredientlist)  // Optional: Load child ingredients
                                        .ThenInclude(ci => ci.Good)  // Optional: Load child's goods
                            .ToListAsync()
                    );
                    foreach (var rec in recipes)
                    {
                        foreach (var ing in rec.Ingredientlist)
                        {
                            ing.GetQuantityInGram(ing.Quantity);
                            ing.ConvertToOtherUnits();
                            ing.AddUnitOptions();
                        }

                        // Optional: Process child recipes' ingredients too
                        foreach (var hierarchy in rec.ChildRecipes)
                        {
                            if (hierarchy.ChildRecipe != null)
                            {
                                foreach (var childIng in hierarchy.ChildRecipe.Ingredientlist)
                                {
                                    childIng.GetQuantityInGram(childIng.Quantity);
                                    childIng.ConvertToOtherUnits();
                                    childIng.AddUnitOptions();
                                }
                            }
                        }
                    }
                    return recipes;
                }
            }
            catch (Exception ex) {
                // Log the exception (you can use any logging framework you prefer)
                Console.WriteLine($"An error occurred while fetching recipes: {ex.Message}");
                return new ObservableCollection<Recipe>();
            }
        }

        public async Task<bool> AddRecipe(Recipe recipe, ObservableCollection<Recipe> recipelist)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    foreach (var ing in recipe.Ingredientlist)
                    {
                        ing.Recipe = recipe;
                        if (ing.Good != null)
                        {
                            context.Entry(ing.Good).State = EntityState.Unchanged;
                        }
                    }

                    // Handle child recipes' ingredients (NEW: Prevents insert errors for ChildRecipes)
                    foreach (var hierarchy in recipe.ChildRecipes)
                    {
                        if (hierarchy.ChildRecipe != null)
                        {
                            foreach (var childIng in hierarchy.ChildRecipe.Ingredientlist)
                            {
                                childIng.Recipe = hierarchy.ChildRecipe;  // Set FK if needed
                                if (childIng.Good != null)
                                {
                                    context.Entry(childIng.Good).State = EntityState.Unchanged;
                                }
                            }
                            // Attach the child recipe itself (if it exists)
                            if (hierarchy.ChildRecipe.Id > 0)
                            {
                                context.Recipes.Attach(hierarchy.ChildRecipe);
                            }
                        }
                    }

                    context.Recipes.Add(recipe);
                    await context.SaveChangesAsync();
                }
                recipelist.Add(recipe);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while adding a recipe: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdateRecipe(Recipe recipe)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var existing = await context.Recipes.Include(r => r.Ingredientlist).FirstAsync(r => r.Id == recipe.Id);
                    context.Entry(existing).CurrentValues.SetValues(recipe);
                    await context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating the recipe: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRecipe(Recipe recipe, ObservableCollection<Recipe> recipelist)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var menuItems = context.MenuItems.Where(m => m.LunchRecipeId == recipe.Id || m.DinnerRecipeId == recipe.Id);

                    foreach (var item in menuItems)
                    {
                        if (item.LunchRecipeId == recipe.Id) item.LunchRecipeId = null;
                        if (item.DinnerRecipeId == recipe.Id) item.DinnerRecipeId = null;
                    }
                    await context.SaveChangesAsync();
                    
                    context.Recipes.Remove(recipe);
                    await context.SaveChangesAsync();
                }
                recipelist.Remove(recipe);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting the recipe: {ex.Message}");
                return false;
            }
        }


    }
}
