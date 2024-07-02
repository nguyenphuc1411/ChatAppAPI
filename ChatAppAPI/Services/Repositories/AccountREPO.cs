using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;

namespace ChatAppAPI.Services.Repositories
{
    public class AccountREPO : IAccountREPO
    {
        private readonly UserManager<ManageUser> userManager;
        private readonly SignInManager<ManageUser> signInManager;
        private readonly IHttpContextAccessor _httpContext;
        public AccountREPO(UserManager<ManageUser> userManager, SignInManager<ManageUser> signInManager, IHttpContextAccessor httpContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _httpContext = httpContext;
        }

        public async Task<ManageUser> GetCurrenUser()
        {
            var email = _httpContext.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<bool> Login(LoginVM login)
        {
            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> Logout()
        {
            await signInManager.SignOutAsync();
            return true;
        }
        public async Task<bool> Register(RegisterVM register)
        {
            var existingUser = await userManager.FindByEmailAsync(register.Email);
            if (existingUser != null)
            {
                return false;
            }
            var user = new ManageUser
            {
                UserName = register.Email,
                FullName = register.FullName,
                Email = register.Email,
                CreatedDate = DateTime.Now
            };
            var result = await userManager.CreateAsync(user,register.Password);
            return result.Succeeded;
        }
    }
}
