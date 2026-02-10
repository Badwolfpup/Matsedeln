using MatsedelnShared;
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matserver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoodsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public GoodsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: api/goods
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetGoods()
    {
        try
        {
            var list = await _db.Goods.ToListAsync();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return Problem("Error retrieving data from database. " + ex.Message);
        }
    }

    // POST: api/goods
    [HttpPost]
    public async Task<ActionResult> PostGood(Goods good)
    {
        try
        {
            if (good.Id == 0) _db.Goods.Add(good);
            else _db.Entry(good).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Problem("Error adding good to the database. " + ex.Message);
        }

        return Ok(good);
    }

    // PUT: api/goods/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGood(int id, Goods good)
    {
        if (id != good.Id) return BadRequest();

        _db.Entry(good).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Goods.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/goods/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGood(int id)
    {
        try
        {
            var good = await _db.Goods.FindAsync(id);
            if (good == null) return NotFound();

            _db.Goods.Remove(good);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem("Error deleting good from the database. " + ex.Message);

        }
    }
}