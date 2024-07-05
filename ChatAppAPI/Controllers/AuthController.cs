using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAccountREPO _repos;
        private readonly IEmailREPO _reposEmail;
        private readonly UserManager<ManageUser> _userManager;
        public AuthController(IConfiguration config, IAccountREPO repos, UserManager<ManageUser> userManager, IEmailREPO reposEmail)
        {
            _config = config;
            _repos = repos;
            _userManager = userManager;
            _reposEmail = reposEmail;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                bool result = await _repos.Login(login);
                if (result)
                {
                    return Ok(new { token = GenerateToken(login) });
                }
                return Unauthorized();
            }
            return BadRequest("Invalid user");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                bool result = await _repos.Register(register);
                if (result)
                {
                    return Ok(new { suceess = true, message = "Registed user successfully" });
                }
                return Unauthorized();
            }
            return BadRequest("Invalid input for user");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _repos.Logout();
            return Ok();
        }
        private string GenerateToken(LoginVM login)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, login.Email),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iss, jwtSettings["Issuer"]),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud, jwtSettings["Audience"]),
            new Claim(ClaimTypes.NameIdentifier,login.Email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [Authorize]
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var email = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (email == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var userInfo = new
            {
                user.FullName,
                user.Email,
                user.CreatedDate,
                user.Avatar
            };

            return Ok(userInfo);
        }   
        
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> SendMailForgotpassword(EmailRequest request)
        {
            bool result = await _reposEmail.SendEmailAsync(request);
            return result ? Ok(request) : BadRequest();
        }
        [HttpPost("changepassword")]
        public async Task<IActionResult> ConfirmChangePassword(ChangePassword newPass)
        {
            bool result = await _repos.ConfirmChangePassword(newPass);
            return result ? Ok("Change password successfully") : BadRequest();
        }
        [Authorize]
        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile([FromForm]string fullName,[FromForm]IFormFile? file)
        {
            bool result = await _repos.UpdateProfile(fullName, file);
            return result ? Ok("Change profile successfully") : BadRequest();
        }
    }
}

