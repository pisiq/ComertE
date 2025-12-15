using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class CartRepository : ICartRepository
    {
        private readonly HotelContext _context;

        public CartRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Location)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Photos)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                CreatedDate = DateTime.Now
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Location)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
        }
    }
}
