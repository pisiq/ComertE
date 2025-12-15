// Data/Restaurant/RestaurantRepository.cs
using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly HotelContext _context;

        public RestaurantRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants
                .Include(r => r.Location)
                .Include(r => r.Dish)
                .ToListAsync();
        }

        public async Task<Restaurant> GetByIdAsync(int id)
        {
            return await _context.Restaurants
                .Include(r => r.Location)
                .Include(r => r.Dish)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Restaurant>> GetByLocationIdAsync(int locationId)
        {
            return await _context.Restaurants
                .Include(r => r.Location)
                .Include(r => r.Dish)
                .Where(r => r.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetByDishIdAsync(int dishId)
        {
            return await _context.Restaurants
                .Include(r => r.Location)
                .Include(r => r.Dish)
                .Where(r => r.DishId == dishId)
                .ToListAsync();
        }

        public async Task AddAsync(Restaurant restaurant)
        {
            try
            {
                Console.WriteLine("Repository: Adding restaurant to database");
                await _context.Restaurants.AddAsync(restaurant);
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync result: {result} records affected");

                if (result <= 0)
                {
                    Console.WriteLine("Warning: SaveChangesAsync returned 0 records affected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository error adding restaurant: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
