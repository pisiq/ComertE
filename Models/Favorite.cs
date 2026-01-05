using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public User? User { get; set; }

        [Required]
        public int RoomId { get; set; }

        public Room? Room { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
