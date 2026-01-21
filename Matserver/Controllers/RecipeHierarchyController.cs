using MatsedelnShared; // Your Shared Library namespace
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matserver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    // The Server injects the DbContext here automatically
    public IngredientsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: api/ingredients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
    {
        return await _db.Ingredients.ToListAsync();
    }

    // POST: api/ingredients
    [HttpPost]
    public async Task<ActionResult<Ingredient>> CreateIngredient(Ingredient ingredient)
    {
        _db.Ingredients.Add(ingredient);
        await _db.SaveChangesAsync();

        // Returns a 201 Created status
        return CreatedAtAction(nameof(GetIngredients), new { id = ingredient.Id }, ingredient);
    }

    // PUT: api/ingredients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient(int id, Ingredient ingredient)
    {
        if (id != ingredient.Id) return BadRequest();

        _db.Entry(ingredient).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Ingredients.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/ingredients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await _db.Ingredients.FindAsync(id);
        if (ingredient == null) return NotFound();

        _db.Ingredients.Remove(ingredient);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}