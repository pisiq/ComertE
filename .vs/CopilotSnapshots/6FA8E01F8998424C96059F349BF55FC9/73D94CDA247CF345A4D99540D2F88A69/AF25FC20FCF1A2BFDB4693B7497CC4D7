// Services/Restaurant/RestaurantService.cs
using Hotel.Data;
using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _restaurantRepository.GetAllAsync();
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            return await _restaurantRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByLocationIdAsync(int locationId)
        {
            return await _restaurantRepository.GetByLocationIdAsync(locationId);
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByDishIdAsync(int dishId)
        {
            return await _restaurantRepository.GetByDishIdAsync(dishId);
        }

        public async Task AddRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                Console.WriteLine($"Adding restaurant with LocationId: {restaurant.LocationId}, DishId: {restaurant.DishId}");
                await _restaurantRepository.AddAsync(restaurant);
                Console.WriteLine("Restaurant added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddRestaurantAsync: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // Rethrow to be handled in controller
            }
        }

        public async Task<bool> UpdateRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                await _restaurantRepository.UpdateAsync(restaurant);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRestaurantAsync(int id)
        {
            try
            {
                await _restaurantRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
