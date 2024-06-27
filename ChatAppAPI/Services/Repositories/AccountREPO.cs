using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;
using ChatAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChatAppAPI.Services.Repositories
{
    public class AccountREPO : IAccountREPO
    {
        private readonly UserManager<ManageUser> userManager;
        private readonly SignInManager<ManageUser> signInManager;

        public AccountREPO(UserManager<ManageUser> userManager, SignInManager<ManageUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
