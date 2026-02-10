using MatsedelnShared;
using MatsedelnShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuEntryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MenuEntryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/menuentry
        [HttpGet("{date}")]
        public async Task<ActionResult<IEnumerable<MenuEntry>>> GetMenuEntry(DateTime date)
        {
            try
            {
                await AddEmptyDays(date);
                var list = await _db.MenuItems.Include(mi => mi.LunchRecipe).Include(mi => mi.DinnerRecipe).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem("Error retrieving data from database. " + ex.Message);
            }
        }

        // POST: api/menuentry
        [HttpPost]
        public async Task<ActionResult> CreateMenuEntry(MenuEntry menu)
        {
            try
            {
                if (menu.Id == 0) _db.MenuItems.Add(menu);
                else _db.Entry(menu).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem("Error adding menuentry to the database. " + ex.Message);
            }

            // Returns a 201 Created status
            return Ok(menu);
        }

        // PUT: api/menuentry/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuEntry(int id, MenuEntry menu)
        {
            if (id != menu.Id) return BadRequest();

            _db.Entry(menu).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.MenuItems.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/menuentry/5
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

        private async Task AddEmptyDays(DateTime date)
        {
            try
            {
                var haslastmonth = await _db.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date.AddMonths(-1));
                var hasthismonth = await _db.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date);
                var hasnextmonth = await _db.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date.AddMonths(1));

                if (haslastmonth != null && hasthismonth != null && hasnextmonth != null) return;
                if (haslastmonth == null)
                {
                    var year = date.AddMonths(-1).Year;
                    var month = date.AddMonths(-1).Month;
                    var daysinmonth = DateTime.DaysInMonth(year, month);
                    var firstday = new DateTime(year, month, 1);
                    for (int i = 0; i < daysinmonth; i++)
                    {
                        var newEntry = new MenuEntry
                        {
                            Date = firstday.AddDays(i)
                        };
                        _db.MenuItems.Add(newEntry);
                    }
                }
                if (hasthismonth == null)
                {
                    var year = date.Year;
                    var month = date.Month;
                    var daysinmonth = DateTime.DaysInMonth(year, month);
                    var firstday = new DateTime(year, month, 1);
                    for (int i = 0; i < daysinmonth; i++)
                    {
                        var newEntry = new MenuEntry
                        {
                            Date = firstday.AddDays(i)
                        };
                        _db.MenuItems.Add(newEntry);
                    }
                }
                if (hasnextmonth == null)
                {
                    var year = date.AddMonths(1).Year;
                    var month = date.AddMonths(1).Month;
                    var daysinmonth = DateTime.DaysInMonth(year, month);
                    var firstday = new DateTime(year, month, 1);
                    for (int i = 0; i < daysinmonth; i++)
                    {
                        var newEntry = new MenuEntry
                        {
                            Date = firstday.AddDays(i)
                        };
                        _db.MenuItems.Add(newEntry);
                    }
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while adding empty days: {ex.Message}");
                return;
            }
        }

    }
}
