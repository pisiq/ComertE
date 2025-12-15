using Hotel.Models;
using Microsoft.AspNetCore.Http;

namespace Hotel.ViewsModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public IFormFile? ProfilePicture { get; set; }

        // Booking lists
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Booking> ActiveBookings { get; set; } = new List<Booking>();
        public List<Booking> PendingBookings { get; set; } = new List<Booking>();
        public List<Booking> PastBookings { get; set; } = new List<Booking>();
        public List<Booking> CancelledBookings { get; set; } = new List<Booking>();
    }
}