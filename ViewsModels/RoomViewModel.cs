using Hotel.Models;
using System.ComponentModel.DataAnnotations;

namespace Hotel.ViewsModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [StringLength(100, ErrorMessage = "Room type cannot exceed 100 characters")]
        [Display(Name = "Room Type")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        [Display(Name = "Price per Night")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Room count is required")]
        [Range(0, 1000, ErrorMessage = "Room count must be between 0 and 1000")]
        [Display(Name = "Available Rooms")]
        public int RoomCount { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        public string? MainPhotoPath { get; set; }

        [Display(Name = "Main Photo")]
        public IFormFile? MainPhoto { get; set; }

        [Display(Name = "Additional Photos")]
        public List<IFormFile>? AdditionalPhotos { get; set; }

        public string? LocationName { get; set; }
        public List<RoomPhotoViewModel>? ExistingPhotos { get; set; }

        public Room ToEntity()
        {
            return new Room
            {
                Id = Id,
                Type = Type,
                Price = Price,
                RoomCount = RoomCount,
                LocationId = LocationId,
                Description = Description,
                MainPhotoPath = MainPhotoPath
            };
        }

        public static RoomViewModel FromEntity(Room room)
        {
            return new RoomViewModel
            {
                Id = room.Id,
                Type = room.Type,
                Price = room.Price,
                RoomCount = room.RoomCount,
                LocationId = room.LocationId,
                Description = room.Description,
                MainPhotoPath = room.MainPhotoPath,
                LocationName = room.Location?.Name,
                ExistingPhotos = room.Photos?.Select(p => new RoomPhotoViewModel
                {
                    Id = p.Id,
                    FilePath = p.FilePath,
                    Caption = p.Caption,
                    IsFeatured = p.IsFeatured,
                    DisplayOrder = p.DisplayOrder
                }).ToList()
            };
        }
    }

    public class RoomPhotoViewModel
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
    }
}