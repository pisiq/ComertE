using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // User who made the booking
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Booking Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Collection of items in this booking
        public List<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

        // Computed properties
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
        public decimal TotalPrice => BookingItems.Sum(item => item.TotalPrice);
    }

    public class BookingItem
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Display(Name = "Price per Night")]
        public decimal PricePerNight { get; set; }

        // Computed property
        public decimal TotalPrice => PricePerNight * Quantity * (Booking?.NumberOfNights ?? 0);
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}