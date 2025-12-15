using Hotel.Models;

namespace Hotel.Data
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task<Cart> CreateCartAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<bool> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> RemoveCartItemAsync(int cartItemId);
        Task<bool> ClearCartAsync(int cartId);
        Task<List<CartItem>> GetCartItemsAsync(int cartId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}
