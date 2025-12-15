using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<Location?> GetLocationByIdAsync(int id);
        Task AddLocationAsync(Location location);
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int id);
    }
}
