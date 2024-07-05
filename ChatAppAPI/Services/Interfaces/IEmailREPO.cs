using ChatAppAPI.Models;

namespace ChatAppAPI.Services.Interfaces
{
    public interface IEmailREPO
    {
        Task<bool> SendEmailAsync(EmailRequest request);
    }
}
