using AutoMapper;
using ChatAppAPI.Data.Context;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace ChatAppAPI.Hubs
{
    public class ChatHub : Hub
    {
        public readonly static List<UserVM> _connection = new List<UserVM>();

        private readonly static Dictionary<string, string> _connectionMap = new Dictionary<string, string>();
        private readonly static Dictionary<string, List<UserVM>> _groupsMap = new Dictionary<string, List<UserVM>>();
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

                foreach (var kvp in _groupsMap)
                {
                    var groupKey = kvp.Key;
                    var groupUsers = kvp.Value;
                    var userDis = groupUsers.FirstOrDefault(x => x.Email == user.Email);
                    if (userDis!=null)
                    {
                        groupUsers.Remove(userDis);
                        var values = _groupsMap
                                .Where(x => x.Key == kvp.Key)
                                .Select(x => x.Value);
                        Clients.Group(groupKey).SendAsync("CurrentUsers", values);
                        break;
                    }
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
                        if (!_groupsMap.ContainsKey(groupName))
                        {
                            _groupsMap[groupName] = new List<UserVM>();
                        }
                        _groupsMap[groupName].Add(user);

                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    }
                    else
                    {
                        // nếu có groups rồi
                        if (_groupsMap.ContainsKey(user.CurrentGroup))
                        {
                            // remover user khoi group
                            _groupsMap[user.CurrentGroup].Remove(user);
                            if (_groupsMap[user.CurrentGroup].Count == 0)
                            {
                                _groupsMap.Remove(user.CurrentGroup);
                            }
                        }
                        if (!_groupsMap.ContainsKey(groupName))
                        {
                            _groupsMap[groupName] = new List<UserVM>();
                        }
                        _groupsMap[groupName].Add(user);


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
                                    Id = item.Id,
                                    Content = item.Content,
                                    FullName = item.Sender.FullName,
                                    Avatar = item.Sender.Avatar,
                                    TimeStamp = item.TimeStamp,
                                    RoomId = item.RoomId,
                                    Email = item.Sender.Email
                                }
                            )
                            .OrderByDescending(x => x.TimeStamp)
                            .Take(20).OrderBy(x => x.TimeStamp).ToListAsync();

                    var values = _groupsMap
                                .Where(x => x.Key == groupName)
                                .Select(x => x.Value);

                    await Clients.Group(groupName).SendAsync("CurrentUsers", values);
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
                    await Clients.Group(room.RoomName).SendAsync("NewMessage", newMessageSend);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("SendMessageFailed", content, roomId.ToString());
            }

        }
        public async Task DeleteMessage(int roomId, int messageId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == GetEmail);
            if (user != null)
            {
                var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == messageId && x.RoomId == roomId);
                if (message != null)
                {
                    _context.Messages.Remove(message);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        var room = await _context.Rooms.FindAsync(roomId);
                        await Clients.Group(room.RoomName).SendAsync("DeleteMessageSuccess", _mapper.Map<MessageVM>(message));
                    }
                }

            }
            else
            {
                await Clients.Caller.SendAsync("DeleteMessageFailed", messageId);
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
