using AutoMapper;
using ChatAppAPI.Data.Context;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppAPI.Services.Repositories
{
    public class RoomREPO : IRoomREPO
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public RoomREPO(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(RoomVM roomVM)
        {
            var room = new Room
            {
                RoomName = roomVM.RoomName,
                AdminId = roomVM.AdminId,
                CreatedDate = DateTime.Now
            };
            _context.Rooms.Add(room);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room!=null)
            {
                _context.Rooms.Remove(room);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<List<RoomVM>> GetAll()
        {
            var listRoom =await _context.Rooms.Include(x=>x.Admin).ToListAsync();
            var listRoomVM = _mapper.Map<List<RoomVM>>(listRoom);
            return listRoomVM;
        }

        public async Task<RoomVM> GetById(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            var roomVM = _mapper.Map<RoomVM>(room);
            return roomVM;
        }

        public async Task<bool> UpdateAsync(int roomId, RoomVM roomVM)
        {
            if (roomId != roomVM.Id)
            {
                return false;
            }
            var room = await _context.Rooms.FindAsync(roomId);
            if (room!=null)
            {
                room.RoomName = roomVM.RoomName;
                room.AdminId = roomVM.AdminId;
                _context.Rooms.Update(room);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
