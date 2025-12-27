using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Matsedeln.Utils;
using Matsedeln.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace Matsedeln.Utils
{
    public class GoodsService
    {

        public async Task<ObservableCollection<Goods>> GetGoods()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var observableGoods = new ObservableCollection<Goods>(context.Goods.ToList());
                    return observableGoods;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while retrieving goods: {ex.Message}");
                return new ObservableCollection<Goods>();
            }
        }

        public async Task<bool> AddGoods(Goods good, ObservableCollection<Goods> goodslist)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Goods.Add(good);
                    context.SaveChanges();
                }
                goodslist.Add(good);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while adding goods: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateGoods(Goods good)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Goods.Update(good);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while updating goods: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteGoods(Goods good, ObservableCollection<Goods> goodlist)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Goods.Remove(good);
                    context.SaveChanges();
                }
                goodlist.Remove(good);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while deleting goods: {ex.Message}");
                return false;
            }
        }
    }
}
