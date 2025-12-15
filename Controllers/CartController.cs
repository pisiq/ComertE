using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login to add items to cart" });
            }

            // Use default dates (tomorrow for 1 night) - will be updated in cart
            var checkInDate = DateTime.Today.AddDays(1);
            var checkOutDate = DateTime.Today.AddDays(2);

            var success = await _cartService.AddToCartAsync(userId, productId, checkInDate, checkOutDate, quantity);

            if (success)
            {
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, message = "Product added to cart", itemCount });
            }

            return Json(new { success = false, message = "Failed to add product to cart" });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _cartService.UpdateCartItemQuantityAsync(userId, cartItemId, quantity);
            
            if (success)
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, total, itemCount });
            }

            return Json(new { success = false, message = "Failed to update quantity" });
        }

        // POST: Cart/UpdateDates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDates(int cartItemId, DateTime checkInDate, DateTime checkOutDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _cartService.UpdateCartItemDatesAsync(userId, cartItemId, checkInDate, checkOutDate);
            
            if (success)
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, total, itemCount });
            }

            return Json(new { success = false, message = "Failed to update dates" });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            
            if (success)
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                TempData["SuccessMessage"] = "Item removed from cart";
                return Json(new { success = true, total, itemCount });
            }

            return Json(new { success = false, message = "Failed to remove item" });
        }

        // POST: Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var success = await _cartService.ClearCartAsync(userId);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Cart cleared successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to clear cart";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/GetItemCount (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var count = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { count });
        }

        // POST: Cart/UpdateAllDates - Update dates for all items in cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAllDates(DateTime checkInDate, DateTime checkOutDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Cart is empty" });
            }

            // Update dates for all cart items
            foreach (var item in cart.CartItems)
            {
                await _cartService.UpdateCartItemDatesAsync(userId, item.Id, checkInDate, checkOutDate);
            }

            var total = await _cartService.GetCartTotalAsync(userId);
            var itemCount = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { success = true, total, itemCount });
        }
    }
}
