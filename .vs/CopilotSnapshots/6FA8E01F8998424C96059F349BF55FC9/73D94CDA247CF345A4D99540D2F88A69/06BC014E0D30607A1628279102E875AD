// Data/Dish/IDishRepository.cs
using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish> GetByIdAsync(int id);
        Task AddAsync(Dish dish);
        Task UpdateAsync(Dish dish);
        Task DeleteAsync(int id);
    }
}
