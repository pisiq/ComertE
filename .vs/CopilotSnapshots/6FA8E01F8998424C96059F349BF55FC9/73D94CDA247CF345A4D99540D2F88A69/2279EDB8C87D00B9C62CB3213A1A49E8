// Services/Dish/DishService.cs
using Hotel.Data;
using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;

        public DishService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
        {
            return await _dishRepository.GetAllAsync();
        }

        public async Task<Dish> GetDishByIdAsync(int id)
        {
            return await _dishRepository.GetByIdAsync(id);
        }

        public async Task AddDishAsync(Dish dish)
        {
            await _dishRepository.AddAsync(dish);
        }

        public async Task<bool> UpdateDishAsync(Dish dish)
        {
            try
            {
                await _dishRepository.UpdateAsync(dish);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteDishAsync(int id)
        {
            try
            {
                await _dishRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
