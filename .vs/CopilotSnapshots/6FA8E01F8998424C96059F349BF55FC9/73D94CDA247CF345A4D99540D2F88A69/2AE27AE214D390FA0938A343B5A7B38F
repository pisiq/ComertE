// Services/Dish/IDishService.cs
using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IDishService
    {
        Task<IEnumerable<Dish>> GetAllDishesAsync();
        Task<Dish> GetDishByIdAsync(int id);
        Task AddDishAsync(Dish dish);
        Task<bool> UpdateDishAsync(Dish dish);
        Task<bool> DeleteDishAsync(int id);
    }
}
