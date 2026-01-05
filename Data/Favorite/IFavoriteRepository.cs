using Hotel.Models;

namespace Hotel.Data
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId);
        Task<Favorite?> GetFavoriteAsync(string userId, int roomId);
        Task<bool> AddFavoriteAsync(Favorite favorite);
        Task<bool> RemoveFavoriteAsync(string userId, int roomId);
        Task<bool> IsFavoriteAsync(string userId, int roomId);
    }
}
