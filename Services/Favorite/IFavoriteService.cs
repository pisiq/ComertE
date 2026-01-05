using Hotel.Models;

namespace Hotel.Services
{
    public interface IFavoriteService
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId);
        Task<bool> AddToFavoritesAsync(string userId, int roomId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int roomId);
        Task<bool> IsFavoriteAsync(string userId, int roomId);
        Task<int> GetFavoriteCountAsync(string userId);
    }
}
