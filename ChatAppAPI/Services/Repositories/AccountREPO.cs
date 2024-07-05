using ChatAppAPI.Data.Context;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;

namespace ChatAppAPI.Services.Repositories
{
    public class AccountREPO : IAccountREPO
    {
        private readonly UserManager<ManageUser> userManager;
        private readonly SignInManager<ManageUser> signInManager;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _environment;
        public AccountREPO(UserManager<ManageUser> userManager, SignInManager<ManageUser> signInManager, IHttpContextAccessor httpContext, AppDbContext context, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _httpContext = httpContext;
            _context = context;
            _environment = environment;
        }

        public async Task<bool> ConfirmChangePassword(ChangePassword newPass)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.TokenForgetPassword==newPass.Token);
            if (user == null) { return false; }
          
            var result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                var result1 = await userManager.AddPasswordAsync(user, newPass.Password);
                if(result1.Succeeded)
                {
                    user.TokenForgetPassword = null;
                    await userManager.UpdateAsync(user);
                }
                return result1.Succeeded;
            }
            else return false;
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

        public async Task<bool> UpdateProfile(string fullName, IFormFile? file)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return false;
            }
            var user = await GetCurrenUser();
            if (file == null)
            {
                if (user == null) return false;
                user.FullName = fullName;
                var result = await userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            string[] allowedExtentions = new[] { ".png", ".jpeg", ".gif", ".jpg", ".svg" };

            var extentionFile = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtentions.Contains(extentionFile))
            {
                return false;
            }
            var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{file.FileName}";
            var path = Path.Combine(_environment.WebRootPath, "Images", fileName);
            try
            {
                if (user.Avatar != null)
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, "Images", user.Avatar);
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                if (user == null) return false;

                user.FullName = fullName;
                user.Avatar = fileName;
                var result = await userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
