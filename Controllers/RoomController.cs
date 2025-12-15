using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hotel.ViewsModels;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly ILocationService _locationService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILocationService locationService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _locationService = locationService;
            _logger = logger;
        }

        // GET: Room
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsWithDetailsAsync();
                return View(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading rooms");
                TempData["ErrorMessage"] = "Error loading rooms.";
                return View(new List<Room>());
            }
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdWithDetailsAsync(id);
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Room not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room details for ID: {RoomId}", id);
                TempData["ErrorMessage"] = "Error loading room details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Room/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateLocationDropdown();
            return View(new RoomViewModel());
        }

        // POST: Room/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel viewModel)
        {
            // Remove validation for navigation properties
            ModelState.Remove("LocationName");
            ModelState.Remove("ExistingPhotos");

            if (ModelState.IsValid)
            {
                try
                {
                    var room = viewModel.ToEntity();
                    await _roomService.AddRoomAsync(room, viewModel.MainPhoto, viewModel.AdditionalPhotos);
                    TempData["SuccessMessage"] = $"Room '{room.Type}' created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating room");
                    ModelState.AddModelError("", "Failed to create room: " + ex.Message);
                }
            }

            await PopulateLocationDropdown();
            return View(viewModel);
        }

        // GET: Room/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdWithDetailsAsync(id);
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Room not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = RoomViewModel.FromEntity(room);
                await PopulateLocationDropdown();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room for edit, ID: {RoomId}", id);
                TempData["ErrorMessage"] = "Error loading room.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                TempData["ErrorMessage"] = "Room ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            // Remove validation for navigation properties
            ModelState.Remove("LocationName");
            ModelState.Remove("ExistingPhotos");

            if (ModelState.IsValid)
            {
                try
                {
                    var room = viewModel.ToEntity();
                    var success = await _roomService.UpdateRoomAsync(room, viewModel.MainPhoto, viewModel.AdditionalPhotos);

                    if (!success)
                    {
                        TempData["ErrorMessage"] = "Room not found or update failed.";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["SuccessMessage"] = $"Room '{room.Type}' updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating room ID: {RoomId}", id);
                    ModelState.AddModelError("", "Failed to update room: " + ex.Message);
                }
            }

            // Repopulate ExistingPhotos when validation fails or error occurs
            try
            {
                var existingRoom = await _roomService.GetRoomByIdWithDetailsAsync(id);
                if (existingRoom != null)
                {
                    viewModel.ExistingPhotos = existingRoom.Photos?.Select(p => new RoomPhotoViewModel
                    {
                        Id = p.Id,
                        FilePath = p.FilePath,
                        Caption = p.Caption,
                        IsFeatured = p.IsFeatured,
                        DisplayOrder = p.DisplayOrder
                    }).ToList();

                    // Preserve MainPhotoPath if not changing it
                    if (viewModel.MainPhoto == null && !string.IsNullOrEmpty(existingRoom.MainPhotoPath))
                    {
                        viewModel.MainPhotoPath = existingRoom.MainPhotoPath;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading existing photos for room ID: {RoomId}", id);
            }

            await PopulateLocationDropdown();
            return View(viewModel);
        }

        // GET: Room/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdWithDetailsAsync(id);
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Room not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading room for delete, ID: {RoomId}", id);
                TempData["ErrorMessage"] = "Error loading room.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Room/DeleteConfirmed
        [HttpPost]
        [ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete room ID: {RoomId}", id);

                var result = await _roomService.DeleteRoomAsync(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Room deleted successfully.";
                    _logger.LogInformation("Successfully deleted room ID: {RoomId}", id);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete room.";
                    _logger.LogWarning("Failed to delete room ID: {RoomId}", id);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Business logic exception - show user-friendly message
                _logger.LogWarning(ex, "Cannot delete room ID: {RoomId} - {Message}", id, ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room ID: {RoomId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the room.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Room/DeletePhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhoto(int id, int photoId)
        {
            try
            {
                _logger.LogInformation("Deleting photo ID: {PhotoId} from room ID: {RoomId}", photoId, id);

                var success = await _roomService.DeleteRoomPhotoAsync(photoId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Photo deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete photo.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo ID: {PhotoId}", photoId);
                TempData["ErrorMessage"] = "Error deleting photo: " + ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        private async Task PopulateLocationDropdown()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            ViewBag.Locations = new SelectList(locations, "Id", "Name");
        }

        // Public view for customers
        public async Task<IActionResult> AllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsWithDetailsAsync();
                return View(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all rooms");
                TempData["ErrorMessage"] = "Error loading rooms.";
                return View(new List<Room>());
            }
        }
    }
}