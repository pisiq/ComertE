using Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Data
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetAllWithDetailsAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<Room?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task AddPhotoAsync(RoomPhoto photo);
        Task UpdatePhotoAsync(RoomPhoto photo);
        Task DeletePhotoAsync(int photoId);
    }
}
