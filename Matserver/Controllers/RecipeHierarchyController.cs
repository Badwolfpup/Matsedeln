using MatsedelnShared; // Your Shared Library namespace
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matserver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeHierarchyController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    // The Server injects the DbContext here automatically
    public RecipeHierarchyController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: api/ingredients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeHierarchy>>> GetRecipeHierarchy()
    {
        return await _db.RecipeHierarchies.ToListAsync();
    }

    // POST: api/ingredients
    [HttpPost]
    public async Task<ActionResult<RecipeHierarchy>> CreateRecipeHierarchy(RecipeHierarchy hierarchy)
    {
        _db.RecipeHierarchies.Add(hierarchy);
        await _db.SaveChangesAsync();

        // Returns a 201 Created status
        return CreatedAtAction(nameof(GetRecipeHierarchy), new { id = hierarchy.Id }, hierarchy);
    }

    // PUT: api/ingredients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipeHierarchy(int id, RecipeHierarchy hierarchy)
    {
        if (id != hierarchy.Id) return BadRequest();

        _db.Entry(hierarchy).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.RecipeHierarchies.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/ingredients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipeHierarchy(int id)
    {
        var hierarchy = await _db.RecipeHierarchies.FindAsync(id);
        if (hierarchy == null) return NotFound();

        _db.RecipeHierarchies.Remove(hierarchy);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}