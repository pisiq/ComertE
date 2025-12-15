using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelContext _context;

        public BookingRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Location)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Photos)
                .OrderByDescending(b => b.BookingDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Location)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Photos)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Location)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                        .ThenInclude(r => r.Photos)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                    .ThenInclude(bi => bi.Room)
                .Where(b => b.BookingItems.Any(bi => bi.RoomId == roomId))
                .OrderByDescending(b => b.BookingDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking.Id;
        }

        public async Task UpdateAsync(Booking booking)
        {
            // Re-attach if not tracked
            var existingBooking = await _context.Bookings.FindAsync(booking.Id);
            if (existingBooking != null)
            {
                _context.Entry(existingBooking).CurrentValues.SetValues(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int quantity = 1)
        {
            // Get the room to check total availability
            var room = await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null || room.RoomCount < quantity)
            {
                return false;
            }

            // Get all confirmed/pending bookings for this room that overlap with the requested dates
            var overlappingBookings = await _context.BookingItems
                .Include(bi => bi.Booking)
                .Where(bi => bi.RoomId == roomId &&
                       bi.Booking.Status != BookingStatus.Cancelled &&
                       bi.Booking.CheckInDate < checkOutDate &&
                       bi.Booking.CheckOutDate > checkInDate)
                .AsNoTracking()
                .ToListAsync();

            // Calculate total quantity already booked
            var bookedQuantity = overlappingBookings.Sum(bi => bi.Quantity);

            // Check if requested quantity is available
            return (room.RoomCount - bookedQuantity) >= quantity;
        }
    }
}