using ChatAppAPI.Hubs;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomREPO _repos;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IAccountREPO _account;
        public RoomsController(IRoomREPO repos, IHubContext<ChatHub> hubContext, IAccountREPO account)
        {
            _repos = repos;
            _hubContext = hubContext;
            _account = account;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var listRoomVM = await _repos.GetAll();
            return Ok(listRoomVM);
        }
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetById(int rooomId)
        {
            var roomVM = await _repos.GetById(rooomId);

            return Ok(roomVM);
        }
        [HttpPost]
        public async Task<IActionResult> Post(RoomVM roomVM )
        {
            bool result = await _repos.CreateAsync(roomVM);
            if(result)
            {
                var user = await _account.GetCurrenUser();
                roomVM.AdminName =user.FullName;
                roomVM.Avatar = user.Avatar;
                await _hubContext.Clients.All.SendAsync("NewGroup",roomVM);
                return Ok(roomVM);
            }
            return BadRequest();
        }
        [HttpPut("{roomId}")]
        public async Task<IActionResult> Put(int roomId,RoomVM roomVM)
        {
            bool result = await _repos.UpdateAsync(roomId,roomVM);
            return result ? Ok(result) : BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int roomId)
        {
            bool result = await _repos.DeleteAsync(roomId);
            return result ? Ok(result) : BadRequest();
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery]string? search)
        {
            var listRoomVM = await _repos.Search(search);
            return Ok(listRoomVM);
        }

    }
}
