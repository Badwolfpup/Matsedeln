using Matsedeln.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Matsedeln.Utils
{
    public class MenuService
    {
        //public AppData Ad { get; } = AppData.Instance;
        public async Task<ObservableCollection<MenuEntry>> GetMenuItems()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var menuItems = new ObservableCollection<MenuEntry>(await context.MenuItems.Include(mi => mi.LunchRecipe).Include(mi => mi.DinnerRecipe).ToListAsync());
                    return menuItems;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging framework you prefer)
                Console.WriteLine($"An error occurred while fetching menu items: {ex.Message}");
                return new ObservableCollection<MenuEntry>();
            }
        }

        public async Task<bool> AddMenuItem(Recipe lunch, Recipe dinner, DateTime date, ObservableCollection<MenuEntry> menuList)
        {
            try
            {
                var menuentry = new MenuEntry(lunch, dinner, date);
                using (var context = new AppDbContext())
                {
                    context.MenuItems.Add(menuentry);
                    await context.SaveChangesAsync();
                }
                menuList.Add(menuentry);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while adding a menu item: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMenuItem(int menuEntryId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var entry = await context.MenuItems.FindAsync(menuEntryId);
                    if (entry == null) return false;

                    context.MenuItems.Remove(entry);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting a menu item: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddEmptyDays(DateTime date)
        {
            try
            {
                using (var context = new AppDbContext())
                {

                    var haslastmonth = await context.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date.AddMonths(-1));
                    var hasthismonth = await context.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date);
                    var hasnextmonth = await context.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date.AddMonths(1));

                    if (haslastmonth != null && hasthismonth != null && hasnextmonth != null) return false;
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
                            context.MenuItems.Add(newEntry);
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
                            context.MenuItems.Add(newEntry);
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
                            context.MenuItems.Add(newEntry);
                        }
                    }
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while adding empty days: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLunchEntry(MenuEntry menuEntry)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var copymenu = new MenuEntry(menuEntry);
                    int? lunchid = copymenu.LunchRecipeId > 0 ? copymenu.LunchRecipeId : null;
                    int? dinnerid = copymenu.DinnerRecipeId > 0 ? copymenu.DinnerRecipeId : null;
                    copymenu.LunchRecipe = null;
                    copymenu.DinnerRecipe = null;
                    copymenu.LunchRecipeId = lunchid;
                    copymenu.DinnerRecipeId = dinnerid;
                    context.MenuItems.Update(copymenu);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating a menu entry: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDinnerEntry(MenuEntry menuEntry)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var copymenu = new MenuEntry(menuEntry);
                    int? lunchid = copymenu.LunchRecipeId > 0 ? copymenu.LunchRecipeId : null;
                    int? dinnerid = copymenu.DinnerRecipeId > 0 ? copymenu.DinnerRecipeId : null;
                    copymenu.LunchRecipe = null;
                    copymenu.DinnerRecipe = null;
                    copymenu.LunchRecipeId = lunchid;
                    copymenu.DinnerRecipeId = dinnerid;
                    context.MenuItems.Update(copymenu);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating a menu entry: {ex.Message}");
                return false;
            }
        }
    }
}
