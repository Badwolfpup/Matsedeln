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
                    var recipes = new ObservableCollection<Recipe>(await context.Recipes.Include(r => r.Ingredientlist).ThenInclude(g => g.Good).ToListAsync());
                    foreach (var rec in recipes)
                    {
                        foreach (var ing in rec.Ingredientlist)
                        {
                            ing.GetQuantityInGram(ing.Quantity);
                            ing.ConvertToOtherUnits(ing.QuantityInGram);
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
                        context.Entry(ing.Good).State = EntityState.Unchanged;
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
