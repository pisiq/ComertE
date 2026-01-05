using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly HotelContext _context;

        public FavoriteRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .Include(f => f.Room)
                    .ThenInclude(r => r.Location)
                .Include(f => f.Room)
                    .ThenInclude(r => r.Photos)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync();
        }

        public async Task<Favorite?> GetFavoriteAsync(string userId, int roomId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RoomId == roomId);
        }

        public async Task<bool> AddFavoriteAsync(Favorite favorite)
        {
            try
            {
                var existing = await GetFavoriteAsync(favorite.UserId, favorite.RoomId);
                if (existing != null)
                {
                    return false;
                }

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, int roomId)
        {
            var favorite = await GetFavoriteAsync(userId, roomId);
            if (favorite == null)
            {
                return false;
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteAsync(string userId, int roomId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.RoomId == roomId);
        }
    }
}
