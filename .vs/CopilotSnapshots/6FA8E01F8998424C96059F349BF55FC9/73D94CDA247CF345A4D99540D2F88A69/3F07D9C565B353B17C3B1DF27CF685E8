// Services/Restaurant/IRestaurantService.cs
using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
        Task<Restaurant> GetRestaurantByIdAsync(int id);
        Task<IEnumerable<Restaurant>> GetRestaurantsByLocationIdAsync(int locationId);
        Task<IEnumerable<Restaurant>> GetRestaurantsByDishIdAsync(int dishId);
        Task AddRestaurantAsync(Restaurant restaurant);
        Task<bool> UpdateRestaurantAsync(Restaurant restaurant);
        Task<bool> DeleteRestaurantAsync(int id);
    }
}
