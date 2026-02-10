using MatsedelnShared;
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            // 1. Ingredients: Mark existing Goods as Unchanged, clear Recipe back-reference
            foreach (var ing in recipe.Ingredientlist)
            {
                ing.Recipe = null;
                if (ing.Good != null && ing.Good.Id > 0)
                {
                    _db.Entry(ing.Good).State = EntityState.Unchanged;
                }
            }

            // 2. Child Recipes: Clear navigation properties, rely only on FKs
            foreach (var h in recipe.ChildRecipes)
            {
                h.ParentRecipe = null;
                h.ChildRecipe = null;
            }

            // 3. Parent Recipes: Clear navigation properties
            foreach (var h in recipe.ParentRecipes)
            {
                h.ParentRecipe = null;
                h.ChildRecipe = null;
            }

            if (recipe.Id == 0)
            {
                _db.Recipes.Add(recipe);
            }
            else
            {
                // Find IDs of ingredients and child recipes that were removed on the client.
                // Use raw ID queries to avoid loading full entities (which would cause
                // tracking conflicts when Update attaches the incoming graph).
                var incomingIngredientIds = recipe.Ingredientlist
                    .Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();
                var removedIngredientIds = await _db.Ingredients
                    .Where(i => i.RecipeId == recipe.Id && !incomingIngredientIds.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync();
                if (removedIngredientIds.Count > 0)
                {
                    _db.Ingredients.RemoveRange(
                        removedIngredientIds.Select(id => new Ingredient { Id = id }));
                }

                var incomingChildIds = recipe.ChildRecipes
                    .Where(h => h.Id > 0).Select(h => h.Id).ToHashSet();
                var removedChildIds = await _db.RecipeHierarchies
                    .Where(h => h.ParentRecipeId == recipe.Id && !incomingChildIds.Contains(h.Id))
                    .Select(h => h.Id)
                    .ToListAsync();
                if (removedChildIds.Count > 0)
                {
                    _db.RecipeHierarchies.RemoveRange(
                        removedChildIds.Select(id => new RecipeHierarchy { Id = id }));
                }

                _db.Recipes.Update(recipe);
            }

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
