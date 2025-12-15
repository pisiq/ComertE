using Hotel.Data;
using Hotel.Models;

namespace Hotel.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICartRepository _cartRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            ICartRepository cartRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _cartRepository = cartRepository;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _bookingRepository.GetBookingsByUserIdAsync(userId);
        }

        public async Task<List<Booking>> GetUserActiveBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings.Where(b => b.Status == BookingStatus.Confirmed && b.CheckOutDate >= DateTime.Now).ToList();
        }

        public async Task<List<Booking>> GetUserPendingBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings.Where(b => b.Status == BookingStatus.Pending).ToList();
        }

        public async Task<List<Booking>> GetUserPastBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings.Where(b =>
                (b.Status == BookingStatus.Confirmed && b.CheckOutDate < DateTime.Now) ||
                b.Status == BookingStatus.Completed).ToList();
        }

        public async Task<List<Booking>> GetUserCancelledBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings.Where(b => b.Status == BookingStatus.Cancelled).ToList();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateBookingFromCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // Validate all cart items have the same dates
            var firstItem = cart.CartItems.First();
            var checkInDate = firstItem.CheckInDate;
            var checkOutDate = firstItem.CheckOutDate;

            if (!cart.CartItems.All(ci => ci.CheckInDate == checkInDate && ci.CheckOutDate == checkOutDate))
            {
                throw new InvalidOperationException("All items must have the same check-in and check-out dates");
            }

            // Validate dates
            if (checkInDate >= checkOutDate)
            {
                throw new ArgumentException("Check-out date must be after check-in date");
            }

            // Check availability for all items
            foreach (var cartItem in cart.CartItems)
            {
                var isAvailable = await IsRoomAvailableAsync(
                    cartItem.ProductId,
                    checkInDate,
                    checkOutDate,
                    cartItem.Quantity);

                if (!isAvailable)
                {
                    var room = await _roomRepository.GetByIdAsync(cartItem.ProductId);
                    throw new InvalidOperationException(
                        $"Product '{room?.Type}' is not available for the selected dates with quantity {cartItem.Quantity}");
                }
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                BookingDate = DateTime.Now,
                Status = BookingStatus.Pending,
                BookingItems = new List<BookingItem>()
            };

            // Add booking items from cart
            foreach (var cartItem in cart.CartItems)
            {
                var room = await _roomRepository.GetByIdAsync(cartItem.ProductId);

                booking.BookingItems.Add(new BookingItem
                {
                    RoomId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PricePerNight = room.Price
                });
            }

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking == null)
                {
                    return false;
                }

                booking.Status = BookingStatus.Cancelled;
                await _bookingRepository.UpdateAsync(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                // Create a new booking object with updated status to avoid tracking issues
                var updatedBooking = new Booking
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    BookingDate = booking.BookingDate,
                    Status = status
                };

                await _bookingRepository.UpdateAsync(updatedBooking);
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error updating booking status: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _bookingRepository.GetBookingsByRoomIdAsync(roomId);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int quantity = 1)
        {
            if (checkInDate >= checkOutDate)
            {
                return false;
            }

            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null || room.RoomCount < quantity)
            {
                return false;
            }

            return await _bookingRepository.IsRoomAvailableAsync(roomId, checkInDate, checkOutDate, quantity);
        }
    }
}