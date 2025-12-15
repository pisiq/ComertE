// Data/Dish/DishRepository.cs
using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public class DishRepository : IDishRepository
    {
        private readonly HotelContext _context;

        public DishRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _context.Dishes.ToListAsync();
        }

        public async Task<Dish> GetByIdAsync(int id)
        {
            return await _context.Dishes.FindAsync(id);
        }

        public async Task AddAsync(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dish = await GetByIdAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
        }
    }
}
