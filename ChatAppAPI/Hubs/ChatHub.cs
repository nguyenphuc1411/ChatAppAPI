using AutoMapper;
using ChatAppAPI.Data.Context;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace ChatAppAPI.Hubs
{
    public class ChatHub : Hub
    {
        public readonly static List<UserVM> _connection = new List<UserVM>();

        private readonly static Dictionary<string, string> _connectionMap = new Dictionary<string, string>();
        private readonly static Dictionary<string, UserVM> _groupsMap = new Dictionary<string, UserVM>();
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ChatHub(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override Task OnConnectedAsync()
        {
            var user = GetUser();
            if (user != null)
            {
                bool isExist = _connection.Contains(user);
                if (!isExist)
                {
                    _connection.Add(user);
                }
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = GetUser();
            if (user != null)
            {
                bool isExist = _connection.Contains(user);
                if (isExist)
                {
                    _connection.Remove(user);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task Join(string groupName)
        {
            if (GetEmail != null)
            {
                var user = _connection.FirstOrDefault(x => x.Email == GetEmail);
                if (user != null)
                {
                    if (user.CurrentGroup == null)
                    {
                        _groupsMap.Add(groupName,GetUser());
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    }
                    else
                    {
                        _groupsMap.Remove(groupName);
                        _groupsMap.Add(groupName,GetUser());
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.CurrentGroup);
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    }

                    user.CurrentGroup = groupName;

                    var listMessage = await _context.Messages
                        .Include(x => x.Sender).Include(x => x.Room)
                        .Where(x => x.Room.RoomName == groupName)
                            .Select(item =>
                                new MessageVM
                                {
                                    Content = item.Content,
                                    FullName = item.Sender.FullName,
                                    Avatar = item.Sender.Avatar,
                                    TimeStamp = item.TimeStamp,
                                    RoomId = item.RoomId,
                                    Email = item.Sender.Email
                                }
                            )
                            .OrderByDescending(x => x.TimeStamp)
                            .Take(6).OrderBy(x=>x.TimeStamp).ToListAsync();
                    await Clients.Group(groupName).SendAsync("CurrentUsers",_groupsMap.Where(x=>x.Key== groupName));
                    await Clients.Group(groupName).SendAsync("GetMessages", listMessage);
                }
            }
        }
        private UserVM GetUser()
        {
            var email = Context?.User?.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (email == null)
            {
                return null;
            }
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) { return null; }
            return _mapper.Map<UserVM>(user);
        }
        public async Task SendMessage(string content, int roomId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == GetEmail);
            if (user != null)
            {
                var newMessage = new Message
                {
                    Content = content,
                    SenderId = user.Id,
                    RoomId = roomId
                };
                _context.Messages.Add(newMessage);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var room = await _context.Rooms.FindAsync(roomId);
                    var newMessageSend = _mapper.Map<MessageVM>(newMessage);        
                    await Clients.Group(room.RoomName).SendAsync("NewMessage",newMessageSend);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("SendMessageFailed", content, roomId.ToString());
            }

        }
        private string GetEmail
        {
            get
            {
                var email = Context?.User?.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (email == null)
                {
                    return null;
                }
                return email;
            }
        }

    }
}
