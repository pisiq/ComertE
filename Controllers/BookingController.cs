using Hotel.Models;
using Hotel.Services;
using Hotel.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IPayPalService _payPalService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            IRoomService roomService,
            IUserService userService,
            ICartService cartService,
            IPayPalService payPalService,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _userService = userService;
            _cartService = cartService;
            _payPalService = payPalService;
            _logger = logger;
        }

        // GET: Booking/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Users");
                }

                var bookings = await _bookingService.GetUserBookingsAsync(userId);
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bookings");
                TempData["ErrorMessage"] = $"Error loading bookings: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Booking/AllBookings - Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all bookings");
                TempData["ErrorMessage"] = $"Error loading bookings: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("Loading booking details for ID: {BookingId}", id);

                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found: {BookingId}", id);
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("MyBookings");
                }

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (booking.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("Unauthorized access to booking {BookingId} by user {UserId}", id, currentUserId);
                    return Forbid();
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking details for ID: {BookingId}", id);
                TempData["ErrorMessage"] = $"Error loading booking details: {ex.Message}";
                return RedirectToAction("MyBookings");
            }
        }

        // GET: Booking/Create - Create from cart
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Users");
                }

                var cart = await _cartService.GetOrCreateCartAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }

                var firstItem = cart.CartItems.First();

                // Get user information
                var user = await _userService.GetUserByIdAsync(userId);

                var viewModel = new BookingViewModel
                {
                    CheckInDate = firstItem.CheckInDate,
                    CheckOutDate = firstItem.CheckOutDate,
                    Email = user?.Email,
                    Phone = user?.Phone.ToString(),
                    Items = cart.CartItems.Select(ci => new BookingItemViewModel
                    {
                        RoomId = ci.ProductId,
                        RoomType = ci.Product.Type,
                        RoomPrice = ci.Product.Price,
                        Quantity = ci.Quantity,
                        LocationName = ci.Product.Location?.Name ?? "N/A",
                        MainPhotoPath = ci.Product.MainPhotoPath,
                        TotalPrice = ci.TotalPrice
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                TempData["ErrorMessage"] = $"Error creating booking: {ex.Message}";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Users");
                }

                var cart = await _cartService.GetOrCreateCartAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }

                var bookingId = await _bookingService.CreateBookingFromCartAsync(userId);

                if (bookingId > 0)
                {
                    await _cartService.ClearCartAsync(userId);
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction("Pay", new { bookingId });
                }

                TempData["ErrorMessage"] = "Failed to create booking.";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", $"Error creating booking: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: Booking/Pay
        public async Task<IActionResult> Pay(int bookingId)
        {
            try
            {
                _logger.LogInformation("Loading payment page for booking {BookingId}", bookingId);

                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found for payment: {BookingId}", bookingId);
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("Profile", "Users");
                }

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (booking.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("Unauthorized payment access to booking {BookingId}", bookingId);
                    return Forbid();
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payment page for booking {BookingId}", bookingId);
                TempData["ErrorMessage"] = $"Error loading payment page: {ex.Message}";
                return RedirectToAction("Profile", "Users");
            }
        }

        // POST: Booking/Delete - Delete pending booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("Profile", "Users");
                }

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (booking.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                // Only allow deletion of pending bookings
                if (booking.Status != BookingStatus.Pending)
                {
                    TempData["ErrorMessage"] = "Only pending bookings can be deleted.";
                    return RedirectToAction("Profile", "Users");
                }

                var result = await _bookingService.CancelBookingAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Booking deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete the booking.";
                }

                return RedirectToAction("Profile", "Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {BookingId}", id);
                TempData["ErrorMessage"] = $"Error deleting booking: {ex.Message}";
                return RedirectToAction("Profile", "Users");
            }
        }

        // POST: Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("Profile", "Users");
                }

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (booking.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var result = await _bookingService.CancelBookingAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Your booking has been cancelled.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to cancel the booking.";
                }

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(AllBookings));
                }

                return RedirectToAction("Profile", "Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                TempData["ErrorMessage"] = $"Error cancelling booking: {ex.Message}";
                return RedirectToAction("Profile", "Users");
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> CreatePayPalOrder([FromBody] PayPalOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Creating PayPal order for booking {BookingId}", request?.BookingId ?? 0);

                if (request == null || request.BookingId <= 0)
                {
                    _logger.LogWarning("Invalid booking ID: {BookingId}", request?.BookingId ?? 0);
                    return Json(new { success = false, message = "Invalid booking ID" });
                }

                var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found: {BookingId}", request.BookingId);
                    return Json(new { success = false, message = "Booking not found" });
                }

                // Validate booking belongs to current user
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (booking.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("Unauthorized PayPal order creation attempt for booking {BookingId} by user {UserId}", request.BookingId, currentUserId);
                    return Json(new { success = false, message = "Unauthorized" });
                }

                // Pass the booking ID to the PayPal service
                var orderId = await _payPalService.CreateOrderAsync(booking.TotalPrice, "USD", request.BookingId);
                _logger.LogInformation("PayPal order created: {OrderId} for booking {BookingId}", orderId, request.BookingId);

                return Json(new { success = true, orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for booking {BookingId}", request?.BookingId ?? 0);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapturePayPalOrder([FromBody] PayPalCaptureRequest request)
        {
            try
            {
                _logger.LogInformation("Capturing PayPal order {OrderId} for booking {BookingId}", request?.OrderId, request?.BookingId ?? 0);

                if (request == null || string.IsNullOrEmpty(request.OrderId) || request.BookingId <= 0)
                {
                    _logger.LogWarning("Invalid capture request: OrderId={OrderId}, BookingId={BookingId}",
                        request?.OrderId, request?.BookingId ?? 0);
                    return Json(new { success = false, message = "Invalid request" });
                }

                _logger.LogInformation("Calling PayPal service to capture order: {OrderId}", request.OrderId);
                var success = await _payPalService.CaptureOrderAsync(request.OrderId);
                _logger.LogInformation("PayPal capture completed: {OrderId}, Success: {Success}", request.OrderId, success);

                if (success)
                {
                    _logger.LogInformation("Updating booking {BookingId} status to Confirmed", request.BookingId);
                    var updated = await _bookingService.UpdateBookingStatusAsync(request.BookingId, BookingStatus.Confirmed);

                    if (updated)
                    {
                        _logger.LogInformation("Payment captured and booking {BookingId} confirmed successfully", request.BookingId);
                        return Json(new { success = true });
                    }
                    else
                    {
                        _logger.LogError("Payment captured but failed to update booking {BookingId} status", request.BookingId);
                        return Json(new { success = false, message = "Payment captured but failed to update booking status" });
                    }
                }

                _logger.LogWarning("Payment capture failed for order {OrderId}", request.OrderId);
                return Json(new { success = false, message = "Payment capture failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order {OrderId} for booking {BookingId}, Exception: {ExceptionType}: {Message}",
                    request?.OrderId, request?.BookingId ?? 0, ex.GetType().Name, ex.Message);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Booking/PaymentSuccess
        [AllowAnonymous]
        public IActionResult PaymentSuccess(int bookingId)
        {
            try
            {
                _logger.LogInformation("Payment success callback for booking {BookingId}, Authenticated: {IsAuth}",
                    bookingId, User.Identity?.IsAuthenticated);

                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("User not authenticated on payment success, redirecting to login");
                    TempData["SuccessMessage"] = "Payment successful! Your booking has been confirmed.";
                    return RedirectToAction("Login", "Users", new { returnUrl = "/Users/Profile" });
                }

                TempData["SuccessMessage"] = "✅ Payment successful! Your booking has been confirmed.";
                return RedirectToAction("Profile", "Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PaymentSuccess for booking {BookingId}", bookingId);
                TempData["ErrorMessage"] = "Payment was successful but please check your bookings.";
                return RedirectToAction("Profile", "Users");
            }
        }

        // GET: Booking/PaymentCancelled
        [AllowAnonymous]
        public IActionResult PaymentCancelled(int bookingId)
        {
            try
            {
                _logger.LogInformation("Payment cancelled callback for booking {BookingId}, Authenticated: {IsAuth}",
                    bookingId, User.Identity?.IsAuthenticated);

                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("User not authenticated on payment cancel, redirecting to login");
                    TempData["ErrorMessage"] = "❌ Payment was cancelled. Please login to try again.";
                    return RedirectToAction("Login", "Users", new { returnUrl = "/Users/Profile" });
                }

                TempData["ErrorMessage"] = "❌ Payment was cancelled. You can try again from the 'Pending Payment' tab.";
                return RedirectToAction("Profile", "Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PaymentCancelled for booking {BookingId}", bookingId);
                TempData["ErrorMessage"] = "An error occurred. Please check your bookings.";
                return RedirectToAction("Profile", "Users");
            }
        }
    }

    // Request models for PayPal
    public class PayPalOrderRequest
    {
        public int BookingId { get; set; }
    }

    public class PayPalCaptureRequest
    {
        public string OrderId { get; set; }
        public int BookingId { get; set; }
    }
}