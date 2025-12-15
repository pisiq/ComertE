using Hotel.Models;
using Hotel.Services;
using Hotel.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: /Locations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return View(locations);
        }

        // GET: Location/AllLocations
        public async Task<IActionResult> AllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return View(locations);
        }

        // POST: /Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LocationViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                TempData["ErrorMessage"] = "Failed to create location. Please check your input.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var location = new Location
                {
                    Name = viewModel.Name.Trim()
                };

                await _locationService.AddLocationAsync(location);
                TempData["SuccessMessage"] = "Location created successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to create location: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Locations/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(LocationViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                TempData["ErrorMessage"] = "Location name cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var location = new Location
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name.Trim()
                };

                var updated = await _locationService.UpdateLocationAsync(location);
                if (!updated)
                {
                    TempData["ErrorMessage"] = "Failed to update location.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Location updated successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating location: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Locations/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _locationService.DeleteLocationAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "This location cannot be deleted because it has rooms assigned to it.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Location deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting location: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
