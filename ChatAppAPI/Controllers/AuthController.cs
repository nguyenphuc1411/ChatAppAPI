using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
        public AuthController(IConfiguration config, IAccountREPO repos)
        {
            _config = config;
            _repos = repos;
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
                    return Ok(new { suceess = true , message = "Registed user successfully"}) ;
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,login.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iss,_config["Jwt:Issuer"]),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud,_config["Jwt:Audience"]),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

