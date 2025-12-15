// Controllers/RestaurantController.cs
using Hotel.Models;
using Hotel.Services;
using Hotel.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILocationService _locationService;
        private readonly IDishService _dishService;

        public RestaurantController(
            IRestaurantService restaurantService,
            ILocationService locationService,
            IDishService dishService)
        {
            _restaurantService = restaurantService;
            _locationService = locationService;
            _dishService = dishService;
        }

        // GET: Restaurant
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync();
            return View(restaurants);
        }

        // GET: Restaurant/MenuByLocation/{locationId}
        public async Task<IActionResult> MenuByLocation(int locationId)
        {
            var location = await _locationService.GetLocationByIdAsync(locationId);
            if (location == null)
            {
                return NotFound();
            }

            var restaurantItems = await _restaurantService.GetRestaurantsByLocationIdAsync(locationId);

            if (!restaurantItems.Any())
            {
                TempData["InfoMessage"] = "No menu items found for this location.";
            }

            // Group dishes by type
            var groupedDishes = restaurantItems
                .Select(r => r.Dish)
                .GroupBy(d => d.Type)
                .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new MenuByLocationViewModel
            {
                Location = location,
                GroupedDishes = groupedDishes
            };

            return View(viewModel);
        }

        // GET: Restaurant/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = await PrepareRestaurantViewModel(new RestaurantViewModel());
            return View(viewModel);
        }


        // POST: Restaurant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RestaurantViewModel viewModel)
        {
            // Debug info
            Console.WriteLine($"Create Restaurant - ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"LocationId: {viewModel.LocationId}, DishId: {viewModel.DishId}");

            // We don't need these for creating a Restaurant entity
            ModelState.Remove("LocationName");
            ModelState.Remove("DishName");
            ModelState.Remove("Locations");
            ModelState.Remove("Dishes");

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Validation error for {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                // If we get here, something failed; redisplay form
                viewModel = await PrepareRestaurantViewModel(viewModel);
                return View(viewModel);
            }

            try
            {
                // Check if LocationId exists
                var location = await _locationService.GetLocationByIdAsync(viewModel.LocationId);
                if (location == null)
                {
                    ModelState.AddModelError("LocationId", "Selected location does not exist");
                    viewModel = await PrepareRestaurantViewModel(viewModel);
                    return View(viewModel);
                }

                // Check if DishId exists
                var dish = await _dishService.GetDishByIdAsync(viewModel.DishId);
                if (dish == null)
                {
                    ModelState.AddModelError("DishId", "Selected dish does not exist");
                    viewModel = await PrepareRestaurantViewModel(viewModel);
                    return View(viewModel);
                }

                // Check if combination already exists
                var existingEntries = await _restaurantService.GetRestaurantsByLocationIdAsync(viewModel.LocationId);
                if (existingEntries.Any(r => r.DishId == viewModel.DishId))
                {
                    ModelState.AddModelError("", "This dish already exists at this location!");
                    viewModel = await PrepareRestaurantViewModel(viewModel);
                    return View(viewModel);
                }

                var restaurant = new Restaurant
                {
                    LocationId = viewModel.LocationId,
                    DishId = viewModel.DishId
                };

                await _restaurantService.AddRestaurantAsync(restaurant);
                TempData["SuccessMessage"] = "Restaurant menu item created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                ModelState.AddModelError("", $"Failed to create restaurant menu item: {ex.Message}");

                viewModel = await PrepareRestaurantViewModel(viewModel);
                return View(viewModel);
            }
        }



        // GET: Restaurant/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var viewModel = new RestaurantViewModel
            {
                Id = restaurant.Id,
                LocationId = restaurant.LocationId,
                DishId = restaurant.DishId
            };

            viewModel = await PrepareRestaurantViewModel(viewModel);
            return View(viewModel);
        }

        // POST: Restaurant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RestaurantViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var restaurant = new Restaurant
                    {
                        Id = viewModel.Id,
                        LocationId = viewModel.LocationId,
                        DishId = viewModel.DishId
                    };

                    var success = await _restaurantService.UpdateRestaurantAsync(restaurant);
                    if (!success)
                    {
                        return NotFound();
                    }

                    TempData["SuccessMessage"] = "Restaurant menu item updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update restaurant menu item: {ex.Message}");
                }
            }

            // If we get here, something failed; redisplay form
            viewModel = await PrepareRestaurantViewModel(viewModel);
            return View(viewModel);
        }

        // POST: Restaurant/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _restaurantService.DeleteRestaurantAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Restaurant menu item deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete restaurant menu item.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting restaurant menu item: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate dropdown lists
        private async Task<RestaurantViewModel> PrepareRestaurantViewModel(RestaurantViewModel viewModel)
        {
            var locations = await _locationService.GetAllLocationsAsync();
            var dishes = await _dishService.GetAllDishesAsync();

            viewModel.Locations = new SelectList(locations, "Id", "Name", viewModel.LocationId);
            viewModel.Dishes = new SelectList(dishes, "Id", "Name", viewModel.DishId);

            return viewModel;
        }

    }
}
