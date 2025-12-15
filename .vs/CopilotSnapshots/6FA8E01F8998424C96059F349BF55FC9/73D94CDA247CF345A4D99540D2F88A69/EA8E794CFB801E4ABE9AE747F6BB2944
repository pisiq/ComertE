using Hotel.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel.ViewsModels
{
    public class AdminDashboardViewModel
    {
        // Statistics
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int TotalLocations { get; set; }
        public int TotalBookings { get; set; }
        public IEnumerable<User> RecentUsers { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Location> Locations { get; set; } = new List<Location>();

        // For CRUD operations
        public Room Room { get; set; } = new Room();
        public Location Location { get; set; } = new Location();
        public string? Message { get; set; }
    }

    public class LocationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location name is required")]
        [Display(Name = "Location Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

}
