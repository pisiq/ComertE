using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelContext _context;

        public RoomRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAllWithDetailsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Location)
                .Include(r => r.Photos)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<Room?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Location)
                .Include(r => r.Photos)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPhotoAsync(RoomPhoto photo)
        {
            _context.RoomPhotos.Add(photo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePhotoAsync(RoomPhoto photo)
        {
            _context.RoomPhotos.Update(photo);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            var photo = await _context.RoomPhotos.FindAsync(photoId);
            if (photo != null)
            {
                _context.RoomPhotos.Remove(photo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
