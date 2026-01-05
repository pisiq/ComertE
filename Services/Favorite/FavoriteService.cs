using Hotel.Data;
using Hotel.Models;

namespace Hotel.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IRoomRepository _roomRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository, IRoomRepository roomRepository)
        {
            _favoriteRepository = favoriteRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await _favoriteRepository.GetUserFavoritesAsync(userId);
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                return false;
            }

            var favorite = new Favorite
            {
                UserId = userId,
                RoomId = roomId,
                AddedDate = DateTime.Now
            };

            return await _favoriteRepository.AddFavoriteAsync(favorite);
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int roomId)
        {
            return await _favoriteRepository.RemoveFavoriteAsync(userId, roomId);
        }

        public async Task<bool> IsFavoriteAsync(string userId, int roomId)
        {
            return await _favoriteRepository.IsFavoriteAsync(userId, roomId);
        }

        public async Task<int> GetFavoriteCountAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return favorites.Count();
        }
    }
}
