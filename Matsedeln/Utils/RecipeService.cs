using Matsedeln.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Text;

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
                    // Handle ingredients: Set FKs and manage Goods state
                    foreach (var ing in recipe.Ingredientlist)
                    {
                        ing.Recipe = recipe;
                        if (ing.Good != null && context.Entry(ing.Good).State == EntityState.Detached)
                        {
                            context.Attach(ing.Good);  // Attach without changing state
                        }
                    }

                    // Handle child recipes
                    foreach (var hierarchy in recipe.ChildRecipes)
                    {
                        if (hierarchy.ChildRecipe != null)
                        {
                            hierarchy.ChildRecipeId = hierarchy.ChildRecipe.Id;  // Ensure FK is set
                            foreach (var childIng in hierarchy.ChildRecipe.Ingredientlist)
                            {
                                childIng.Recipe = hierarchy.ChildRecipe;
                                if (childIng.Good != null)
                                {
                                    try
                                    {
                                        if (context.Entry(childIng.Good).State == EntityState.Detached)
                                        {
                                            context.Attach(childIng.Good);
                                        }
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        // Entity is already tracked; skip or replace
                                        var existing = context.Goods.Local.FirstOrDefault(g => g.Id == childIng.Good.Id);
                                        if (existing != null) childIng.Good = existing;
                                    }
                                }
                            }
                            // Use Update for existing, Add for new
                            if (hierarchy.ChildRecipe.Id > 0)
                            {
                                context.Recipes.Update(hierarchy.ChildRecipe);
                            }
                            else
                            {
                                context.Recipes.Add(hierarchy.ChildRecipe);
                            }
                        }
                    }

                    // Use Update if recipe exists, Add if new
                    if (recipe.Id > 0)
                    {
                        context.Recipes.Update(recipe);
                    }
                    else
                    {
                        context.Recipes.Add(recipe);
                    }

                    await context.SaveChangesAsync();
                }
                recipelist.Add(recipe);
                return true;
            }
            catch (Exception ex)
            {
                // Use a proper logger instead of Console
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
                    var existing = await context.Recipes.Include(r => r.Ingredientlist).Include(r => r.ChildRecipes).FirstAsync(r => r.Id == recipe.Id);
                    foreach (var ing in recipe.Ingredientlist)
                    {
                        var existingIng = existing.Ingredientlist.FirstOrDefault(i => i.Id == ing.Id);
                        if (existingIng != null)
                        {
                            context.Entry(existingIng).CurrentValues.SetValues(ing);
                        }
                        else
                        {
                            ing.RecipeId = existing.Id;  // Ensure FK is set
                            existing.Ingredientlist.Add(ing);
                        }
                    }
                        var deletedIngredient = existing.Ingredientlist.Where(x => !recipe.Ingredientlist.Any(y => y.Id == x.Id));
                        context.Ingredients.RemoveRange(deletedIngredient);
                    foreach (var rec in recipe.ChildRecipes)
                    {
                        var existingRec = existing.ChildRecipes.FirstOrDefault(i => i.Id == rec.Id);
                        if (existingRec != null)
                        {
                            context.Entry(existingRec).CurrentValues.SetValues(rec);
                        }
                        else
                        {
                            existing.ChildRecipes.Add(rec);
                        }
                    }
                        var deletedRecipe = existing.ChildRecipes.Where(x => !recipe.ChildRecipes.Any(y => y.Id == x.Id));
                        context.RecipeHierarchies.RemoveRange(deletedRecipe);
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
