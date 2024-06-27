using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        [HttpPost]
        public async Task<ActionResult> Post(IFormFile file)
        {
            if(file == null || file.Length <= 0 )
            {
                return BadRequest("File is not null here");
            }
            string[] allowedExtentions = new [] {".png",".jpeg",".gif",".jpg", ".svg" }; 

            var extentionFile = Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!allowedExtentions.Contains(extentionFile) )
            {
                return BadRequest("Invalid file extentions. Allowed extentions are .png, .jpg, .jpeg, .gif, .svg");
            }
            var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{file.FileName}";
            var path = Path.Combine(_environment.WebRootPath, "Images",fileName);

            using(var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }
            return Ok( new { path });
        }

    }
}
