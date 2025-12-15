using Hotel.Data;
using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Hotel.Models.Context;

namespace Hotel.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly HotelContext _context;

        public LocationService(ILocationRepository locationRepository, HotelContext context)
        {
            _locationRepository = locationRepository;
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _locationRepository.GetAllAsync();
        }

        public async Task<Location?> GetLocationByIdAsync(int id)
        {
            return await _locationRepository.GetByIdAsync(id);
        }

        public async Task AddLocationAsync(Location location)
        {
            await _locationRepository.AddAsync(location);
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(location.Id);
            if (existingLocation == null)
                return false;

            existingLocation.Name = location.Name;
            await _locationRepository.UpdateAsync(existingLocation);
            return true;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            // Check if any rooms are using this location
            var hasRooms = _context.Rooms.Any(r => r.LocationId == id);
            if (hasRooms)
                return false; // Can't delete a location that has rooms

            await _locationRepository.DeleteAsync(id);
            return true;
        }
    }
}
