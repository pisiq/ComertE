using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(IFavoriteService favoriteService, ILogger<FavoriteController> logger)
        {
            _favoriteService = favoriteService;
            _logger = logger;
        }

        // GET: Favorite
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            try
            {
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
                return View(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading favorites for user {UserId}", userId);
                TempData["ErrorMessage"] = "Error loading favorites.";
                return View(new List<Hotel.Models.Favorite>());
            }
        }

        // POST: Favorite/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int roomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login to add favorites" });
            }

            try
            {
                var success = await _favoriteService.AddToFavoritesAsync(userId, roomId);
                
                if (success)
                {
                    var count = await _favoriteService.GetFavoriteCountAsync(userId);
                    return Json(new { success = true, message = "Added to favorites", count });
                }

                return Json(new { success = false, message = "Already in favorites or room not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding room {RoomId} to favorites for user {UserId}", roomId, userId);
                return Json(new { success = false, message = "Failed to add to favorites" });
            }
        }

        // POST: Favorite/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int roomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var success = await _favoriteService.RemoveFromFavoritesAsync(userId, roomId);
                
                if (success)
                {
                    var count = await _favoriteService.GetFavoriteCountAsync(userId);
                    return Json(new { success = true, message = "Removed from favorites", count });
                }

                return Json(new { success = false, message = "Favorite not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing room {RoomId} from favorites for user {UserId}", roomId, userId);
                return Json(new { success = false, message = "Failed to remove from favorites" });
            }
        }

        // GET: Favorite/IsFavorite
        [HttpGet]
        public async Task<IActionResult> IsFavorite(int roomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { isFavorite = false });
            }

            try
            {
                var isFavorite = await _favoriteService.IsFavoriteAsync(userId, roomId);
                return Json(new { isFavorite });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking favorite status for room {RoomId}", roomId);
                return Json(new { isFavorite = false });
            }
        }
    }
}
