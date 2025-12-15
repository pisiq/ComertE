using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]  // ← ADD THIS LINE
        public decimal Price { get; set; }

        [Required]
        public int RoomCount { get; set; }

        public string? Description { get; set; }

        public string? MainPhotoPath { get; set; }

        public List<RoomPhoto> Photos { get; set; } = new List<RoomPhoto>();

        [Required]
        public int LocationId { get; set; }

        public Location? Location { get; set; }
    }
}

namespace Hotel.Models
{
    public class RoomPhoto
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public bool IsFeatured { get; set; }

        public int DisplayOrder { get; set; }
    }
}