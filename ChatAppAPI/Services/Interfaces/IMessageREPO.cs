using ChatAppAPI.Models;

namespace ChatAppAPI.Services.Interfaces
{
    public interface IMessageREPO
    {
        Task<List<MessageVM>> GetMessageFromRoom(int roomId);

        Task<bool> DeleteAsync(int messageId);
    }
}
