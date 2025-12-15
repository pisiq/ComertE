// Data/Restaurant/IRestaurantRepository.cs
using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public interface IRestaurantRepository
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<Restaurant> GetByIdAsync(int id);
        Task<IEnumerable<Restaurant>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<Restaurant>> GetByDishIdAsync(int dishId);
        Task AddAsync(Restaurant restaurant);
        Task UpdateAsync(Restaurant restaurant);
        Task DeleteAsync(int id);
    }
}
