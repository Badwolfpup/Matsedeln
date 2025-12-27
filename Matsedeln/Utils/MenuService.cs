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
                    var entry = await context.MenuItems.FirstOrDefaultAsync(me => me.Date.Date == date.Date);
                    if (entry != null) return false;
                    for (int i = 0; i < 7; i++)
                    {
                        var newEntry = new MenuEntry
                        {
                            Date = date.AddDays(i)
                        };
                        context.MenuItems.Add(newEntry);
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
                    if (menuEntry.LunchRecipeId == 0)
                    {
                        menuEntry.LunchRecipe = null;
                    }
                    context.MenuItems.Update(menuEntry);
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
                    if (menuEntry.DinnerRecipeId == 0)
                    {
                        menuEntry.DinnerRecipe = null;
                    }
                    context.MenuItems.Update(menuEntry);
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
