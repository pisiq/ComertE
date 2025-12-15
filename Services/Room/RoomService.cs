using Hotel.Data;
using Hotel.Models;
using Hotel.Models.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly HotelContext _context;
        private readonly ILogger<RoomService> _logger;
        private const string PhotosDirectory = "images/rooms";

        public RoomService(
            IRoomRepository roomRepository,
            IWebHostEnvironment environment,
            HotelContext context,
            ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _environment = environment;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Room>> GetAllRoomsWithDetailsAsync()
        {
            return await _roomRepository.GetAllWithDetailsAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _roomRepository.GetByIdAsync(id);
        }

        public async Task<Room?> GetRoomByIdWithDetailsAsync(int id)
        {
            return await _roomRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<int> AddRoomAsync(Room room, IFormFile? mainPhoto, List<IFormFile>? additionalPhotos)
        {
            // Upload main photo if provided
            if (mainPhoto != null && mainPhoto.Length > 0)
            {
                room.MainPhotoPath = SavePhoto(mainPhoto);
            }

            // Save the room first
            await _roomRepository.AddAsync(room);

            // Upload additional photos if provided
            if (additionalPhotos != null && additionalPhotos.Count > 0)
            {
                SaveAdditionalPhotos(room.Id, additionalPhotos);
            }

            return room.Id;
        }

        public async Task<bool> UpdateRoomAsync(Room room, IFormFile? mainPhoto, List<IFormFile>? additionalPhotos)
        {
            var existingRoom = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == room.Id);
                
            if (existingRoom == null)
            {
                return false;
            }

            // Upload new main photo if provided
            if (mainPhoto != null && mainPhoto.Length > 0)
            {
                room.MainPhotoPath = SavePhoto(mainPhoto);
            }
            else
            {
                room.MainPhotoPath = existingRoom.MainPhotoPath;
            }

            // Update the room
            await _roomRepository.UpdateAsync(room);

            // Upload additional photos if provided
            if (additionalPhotos != null && additionalPhotos.Count > 0)
            {
                SaveAdditionalPhotos(room.Id, additionalPhotos);
            }

            return true;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetByIdWithDetailsAsync(id);
            if (room == null)
            {
                return false;
            }

            // Check for active bookings
            var hasActiveBookings = await _context.BookingItems
                .Include(bi => bi.Booking)
                .AnyAsync(bi => bi.RoomId == id &&
                               (bi.Booking.Status == BookingStatus.Confirmed ||
                                bi.Booking.Status == BookingStatus.Pending));

            if (hasActiveBookings)
            {
                throw new InvalidOperationException("Cannot delete room with active bookings.");
            }

            // Delete inactive booking items
            var inactiveBookings = await _context.BookingItems
                .Include(bi => bi.Booking)
                .Where(bi => bi.RoomId == id &&
                            (bi.Booking.Status == BookingStatus.Cancelled ||
                             bi.Booking.Status == BookingStatus.Completed))
                .ToListAsync();

            if (inactiveBookings.Any())
            {
                _context.BookingItems.RemoveRange(inactiveBookings);
                await _context.SaveChangesAsync();
            }

            // Delete the room
            await _roomRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateRoomPhotoAsync(RoomPhoto photo, IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                photo.FilePath = SavePhoto(file);
            }

            await _roomRepository.UpdatePhotoAsync(photo);
            return true;
        }

        public async Task<bool> DeleteRoomPhotoAsync(int photoId)
        {
            await _roomRepository.DeletePhotoAsync(photoId);
            return true;
        }

        // SIMPLIFIED PHOTO SAVE - NO ERROR HANDLING, JUST BASIC SAVE
        private string SavePhoto(IFormFile file)
        {
            // Create directory
            var uploadPath = Path.Combine(_environment.WebRootPath, PhotosDirectory);
            Directory.CreateDirectory(uploadPath);

            // Create simple filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return web path
            return $"/{PhotosDirectory}/{fileName}";
        }

        // SIMPLIFIED ADDITIONAL PHOTOS
        private void SaveAdditionalPhotos(int roomId, List<IFormFile> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > 0)
                {
                    var filePath = SavePhoto(files[i]);

                    var photo = new RoomPhoto
                    {
                        RoomId = roomId,
                        FilePath = filePath,
                        DisplayOrder = i + 1
                    };

                    _context.RoomPhotos.Add(photo);
                }
            }
            _context.SaveChanges();
        }
    }
}