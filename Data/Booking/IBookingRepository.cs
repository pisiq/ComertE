using Hotel.Models;

namespace Hotel.Data
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<int> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int quantity = 1);
    }
}