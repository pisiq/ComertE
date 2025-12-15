using Hotel.Models;
using Microsoft.AspNetCore.Http;

namespace Hotel.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<IEnumerable<Room>> GetAllRoomsWithDetailsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room?> GetRoomByIdWithDetailsAsync(int id);
        Task<int> AddRoomAsync(Room room, IFormFile? mainPhoto, List<IFormFile>? additionalPhotos);
        Task<bool> UpdateRoomAsync(Room room, IFormFile? mainPhoto, List<IFormFile>? additionalPhotos);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> UpdateRoomPhotoAsync(RoomPhoto photo, IFormFile? file);
        Task<bool> DeleteRoomPhotoAsync(int photoId);
    }
}