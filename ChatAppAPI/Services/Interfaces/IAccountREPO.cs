using ChatAppAPI.Models;

namespace ChatAppAPI.Services.Interfaces
{
    public interface IAccountREPO
    {
        Task<bool> Login(LoginVM login);
        Task<bool> Register(RegisterVM register);
        Task<bool> Logout();
    }
}
