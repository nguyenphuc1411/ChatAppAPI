using AutoMapper;
using ChatAppAPI.Data.Context;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatAppAPI.Services.Repositories
{
    public class RoomREPO : IRoomREPO
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        public RoomREPO(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<bool> CreateAsync(RoomVM roomVM)
        {
            var email = _httpContext.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var admin = await _context.Users.FirstOrDefaultAsync(x=>x.Email == email);
            if (admin == null) { return false; }
            string adminId = admin.Id;
            var room = new Room
            {
                RoomName =roomVM.RoomName,
                AdminId = adminId,
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

        public async Task<List<RoomVM>> Search(string? search)
        {
            var room = new List<Room>();
            if (search != null)
            {
                room = await _context.Rooms.Include(x=>x.Admin).Where(x => x.RoomName.ToLower().Contains(search.ToLower())).ToListAsync();
            }
            else
            {
                room = await _context.Rooms.Include(x=>x.Admin).ToListAsync();
            }
            var roomVM = _mapper.Map<List<RoomVM>>(room);
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
