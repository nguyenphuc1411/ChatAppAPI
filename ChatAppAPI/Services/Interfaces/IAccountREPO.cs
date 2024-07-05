using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;

namespace ChatAppAPI.Services.Interfaces
{
    public interface IAccountREPO
    {
        Task<bool> Login(LoginVM login);
        Task<bool> Register(RegisterVM register);
        Task<bool> Logout();
        Task<ManageUser> GetCurrenUser();
        Task<bool> ConfirmChangePassword(ChangePassword newPass);
        Task<bool> UpdateProfile(string fullName,IFormFile? file);
    }
}
