using Hotel.Models;

namespace Hotel.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetUserBookingsAsync(string userId);
        Task<List<Booking>> GetUserActiveBookingsAsync(string userId);
        Task<List<Booking>> GetUserPendingBookingsAsync(string userId);
        Task<List<Booking>> GetUserPastBookingsAsync(string userId);
        Task<List<Booking>> GetUserCancelledBookingsAsync(string userId);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<int> CreateBookingFromCartAsync(string userId);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int quantity = 1);
    }
}