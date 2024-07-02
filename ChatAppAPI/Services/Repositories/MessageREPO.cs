using AutoMapper;
using ChatAppAPI.Data.Context;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppAPI.Services.Repositories
{
    public class MessageREPO : IMessageREPO
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public MessageREPO(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null) return false;
            _context.Messages.Remove(message);
            var result = await _context.SaveChangesAsync();
            return result > 0;        }

        public async Task<List<MessageVM>> GetMessageFromRoom(int roomId)
        {
            var messages = await
                _context.Messages.Where(x => x.RoomId == roomId)
                .Include(x => x.Room).Include(x => x.Sender)
                .OrderByDescending(x => x.TimeStamp)
                .Take(10).ToListAsync();

            var messeagesVM = _mapper.Map<List<MessageVM>>(messages);

            return messeagesVM;
        }
    }
}
