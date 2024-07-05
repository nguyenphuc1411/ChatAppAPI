using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageREPO _repos;

        public MessagesController(IMessageREPO repos)
        {
            _repos = repos;
        }
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> Get(int roomId)
        {
            var listMessage = await _repos.GetMessageFromRoom(roomId);
            return Ok(listMessage);
        }
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> Delete([FromRoute] int messageId)
        {
            var listMessage = await _repos.GetMessageFromRoom(messageId);
            return Ok(listMessage);
        }
    }
}
