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
            // Check if the entity is already tracked
            var existingEntity = _context.Rooms.Local.FirstOrDefault(r => r.Id == room.Id);
            
            if (existingEntity != null)
            {
                // Detach the existing tracked entity
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            // Attach and mark as modified
            _context.Rooms.Attach(room);
            _context.Entry(room).State = EntityState.Modified;
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
            // Check if the entity is already tracked
            var existingEntity = _context.RoomPhotos.Local.FirstOrDefault(p => p.Id == photo.Id);
            
            if (existingEntity != null)
            {
                // Detach the existing tracked entity
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.RoomPhotos.Attach(photo);
            _context.Entry(photo).State = EntityState.Modified;
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
