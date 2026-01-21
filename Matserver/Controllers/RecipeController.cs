using MatsedelnShared;
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;

namespace Matserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public RecipeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            try
            {
                var recipes = await _db.Recipes
                    .Include(r => r.Ingredientlist).ThenInclude(i => i.Good)
                    .Include(r => r.ChildRecipes).ThenInclude(cr => cr.ChildRecipe)
                        .ThenInclude(child => child.Ingredientlist).ThenInclude(ci => ci.Good)
                    .ToListAsync();

                // Simplify: One loop that calls a helper
                foreach (var recipe in recipes)
                {
                    PrepareRecipe(recipe);
                }

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // Helper to handle the logic in one place
        private void PrepareRecipe(Recipe recipe)
        {
            if (recipe == null) return;

            foreach (var ing in recipe.Ingredientlist) ing.Initialize();

            foreach (var h in recipe.ChildRecipes.Where(x => x.ChildRecipe != null))
            {
                PrepareRecipe(h.ChildRecipe!); // Recursive call handles nested children!
            }
        }

        // POST: api/recipe
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            // 1. Ingredients: Existing Goods stay Unchanged
            foreach (var ing in recipe.Ingredientlist.Where(i => i.Good != null && i.Good.Id > 0))
            {
                _db.Entry(ing.Good!).State = EntityState.Unchanged;
            }

            // 2. Child Recipes: The LINK is new, but the ACTUAL RECIPE is existing
            foreach (var h in recipe.ChildRecipes.Where(c => c.ChildRecipe != null && c.ChildRecipe.Id > 0))
            {
                // Tell EF: "I'm linking to this recipe, don't try to insert a new one"
                _db.Entry(h.ChildRecipe!).State = EntityState.Unchanged;
            }

            // 3. Parent Recipes (Same logic if you allow editing from this side)
            foreach (var h in recipe.ParentRecipes.Where(p => p.ParentRecipe != null && p.ParentRecipe.Id > 0))
            {
                _db.Entry(h.ParentRecipe!).State = EntityState.Unchanged;
            }

            // 4. Finally, Add or Update the main Recipe
            if (recipe.Id == 0)
                _db.Recipes.Add(recipe);
            else
                _db.Recipes.Update(recipe);

            await _db.SaveChangesAsync();
            return Ok(recipe);
        }

        // DELETE: api/recipe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuEntry(int id)
        {
            try
            {
                var menu = await _db.MenuItems.FindAsync(id);
                if (menu == null) return NotFound();

                _db.MenuItems.Remove(menu);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem("Error deleting menu entry from the database. " + ex.Message);

            }
        }
    }
}
