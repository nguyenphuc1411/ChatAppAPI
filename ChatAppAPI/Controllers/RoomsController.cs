using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomREPO _repos;

        public RoomsController(IRoomREPO repos)
        {
            _repos = repos;
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
            return result ? Ok(result) : BadRequest();
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
    }
}
