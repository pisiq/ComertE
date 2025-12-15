
using System.Diagnostics;
using Hotel.Services;
using Hotel.ViewsModels;
using Hotel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Hotel.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly IRoomService _roomService;
        private readonly ILocationService _locationService;
        private readonly IBookingService _bookingService;
        private readonly IDishService _dishService; 
        private readonly IRestaurantService _restaurantService;

        public UsersController(
            IUserService userService,
            SignInManager<User> signInManager,
            IRoomService roomService,
            ILocationService locationService,
            IBookingService bookingService,
            IDishService dishService,
            IRestaurantService restaurantService) // Add this parameter
        {
            _userService = userService;
            _signInManager = signInManager;
            _roomService = roomService;
            _locationService = locationService;
            _bookingService = bookingService;
            _dishService = dishService; // Add this line
            _restaurantService = restaurantService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(ProfileViewModel model)
        {
            if (model.ProfilePicture == null)
            {
                return RedirectToAction("Profile");
            }

            var email = User.Identity.Name;
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            // Process uploaded file
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                // Get file extension
                var fileExtension = Path.GetExtension(model.ProfilePicture.FileName).ToLowerInvariant();

                // Only accept certain file types
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ProfilePicture", "Only image files (.jpg, .jpeg, .png, .gif) are allowed");
                    return RedirectToAction("Profile");
                }

                // Create a unique filename
                var fileName = $"{user.Id}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", fileName);

                // Ensure directory exists
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles"));

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Update database with new path
                user.ProfilePicturePath = $"/images/profiles/{fileName}";
                await _userService.UpdateUserAsync(user);
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize] // Add this attribute to ensure only logged-in users can access their profile
        public async Task<IActionResult> Profile()
        {
            // Get the current user's email from claims
            var email = User.Identity.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            // Fetch user data from your service
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            // Get the user's bookings
            var bookings = await _bookingService.GetUserBookingsAsync(user.Id);

            // Map user data to ProfileViewModel
            var viewModel = new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.Name?.Split(' ').FirstOrDefault() ?? "",
                LastName = user.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                ProfilePicturePath = user.ProfilePicturePath,
                Bookings = bookings
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Use SignInManager directly for clearer diagnostics
            var user = await _userService.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Sign()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(SignModel model)
        {
            try
            {
                // Check model validation
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Try to parse phone number
                int phoneNumber;
                if (!int.TryParse(model.Phone.ToString(), out phoneNumber))
                {
                    ModelState.AddModelError("Phone", "Please enter a valid number without any symbols or spaces");
                    return View(model);
                }

                // Register the user
                var result = await _userService.RegisterUserAsync(model);

                if (result)
                {
                    // Success
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // If registration failed, add a general error
                    ModelState.AddModelError("", "Registration failed. The email may already be registered or the password doesn't meet requirements.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Sign action: {ex.Message}");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var rooms = await _roomService.GetAllRoomsAsync();
                var locations = await _locationService.GetAllLocationsAsync();
                var bookings = await _bookingService.GetAllBookingsAsync();
                var dishes = await _dishService.GetAllDishesAsync();
                var restaurants = await _restaurantService.GetAllRestaurantsAsync(); // Add this line

                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = users.Count(),
                    TotalRooms = rooms.Count(),
                    TotalLocations = locations.Count(),
                    TotalBookings = bookings.Count(),
                    TotalDishes = dishes.Count(),
                    TotalRestaurants = restaurants.Count(), // Add this line
                    RecentUsers = users.OrderByDescending(u => u.Id).Take(5).ToList(),
                    Rooms = rooms.ToList(),
                    Locations = locations.ToList(),
                    Room = new Room(),
                    Location = new Location()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Admin action: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}