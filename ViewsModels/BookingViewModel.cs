using System;
using System.ComponentModel.DataAnnotations;

namespace Hotel.ViewsModels
{
    public class BookingViewModel
    {
        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        // List of items to book (from cart)
        public List<BookingItemViewModel> Items { get; set; } = new List<BookingItemViewModel>();

        // Helper properties
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
        public decimal Subtotal => Items.Sum(item => item.TotalPrice);
        public decimal Tax => Subtotal * 0.1m;
        public decimal TotalPrice => Subtotal + Tax;

        public int BookingId { get; set; }
    }

    public class BookingItemViewModel
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public decimal RoomPrice { get; set; }
        public int Quantity { get; set; }
        public string LocationName { get; set; }
        public string MainPhotoPath { get; set; }

        public decimal TotalPrice { get; set; }
    }
}