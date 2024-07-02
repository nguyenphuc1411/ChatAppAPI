using ChatAppAPI.Models;

namespace ChatAppAPI.Services.Interfaces
{
    public interface IRoomREPO
    {
        Task<List<RoomVM>> GetAll();
        Task<List<RoomVM>> Search(string? search);
        Task<RoomVM> GetById(int roomId);
        Task<bool> CreateAsync(RoomVM roomVM);
        Task<bool> UpdateAsync(int roomId,RoomVM roomVM);
        Task<bool> DeleteAsync(int roomId);
    }
}
