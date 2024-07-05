using ChatAppAPI.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using ChatAppAPI.Models;
using Microsoft.AspNetCore.Identity;
using ChatAppAPI.Data.Entities;

namespace ChatAppAPI.Services.Repositories
{
    public class EmailREPO:IEmailREPO
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ManageUser> _userManager;
        public EmailREPO(IConfiguration config, UserManager<ManageUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<bool> SendEmailAsync(EmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.To);
                if (user == null)
                {
                    return false;
                }

                var fromAddress = new MailAddress(_config["Smtp:SenderEmail"], _config["Smtp:SenderName"]);
                var toAddress = new MailAddress(request.To);

                using var smtpClient = new SmtpClient
                {
                    Host = _config["Smtp:Server"],
                    Port = int.Parse(_config["Smtp:Port"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_config["Smtp:SenderEmail"], _config["Smtp:Password"])
                };
                string random = Guid.NewGuid().ToString();
                user.TokenForgetPassword = random;
                await _userManager.UpdateAsync(user);

                request.ResetLink = $"{request.ResetLink}?token={random}&email={request.To}";

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = request.Subject,
                    Body = RenderBody(request.To, request.ResetLink),
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error sending email: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error sending email: {ex.Message}");
                return false;
            }
        }

        private string RenderBody(string email, string resetLink)
        {
            var body = $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"">
        <title>Reset Your Password</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }}
            .container {{
                width: 100%;
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
                border: 1px solid #ddd;
                border-radius: 5px;
            }}
            .header {{
                background-color: #f7f7f7;
                padding: 10px;
                text-align: center;
                border-bottom: 1px solid #ddd;
            }}
            .content {{
                padding: 20px;
            }}
            .button {{
                display: inline-block;
                padding: 10px 20px;
                margin: 20px 0;
                font-weight: bold;
                background-color: #99FFFF; 
                text-decoration: none;
                border-radius: 5px;
            }}
            .footer {{
                margin-top: 20px;
                font-size: 0.9em;
                text-align: center;
                color: #666;
            }}
        </style>
        </head>
        <body>
        <div class=""container"">
            <div class=""header"">
                <h2>Reset Your Password</h2>
            </div>
            <div class=""content"">
                <p>Hi {email},</p>
                <p>You recently requested to reset your password for your account. Click the button below to reset it.</p>
                <a href=""{resetLink}"" class=""button"">Reset Password</a>
                <p>If you did not request a password reset, please ignore this email or contact support if you have questions.</p>
                <p>Thanks,<br>The Chat App By Thanh Phuc</p>
            </div>
            <div class=""footer"">
                <p>If you’re having trouble clicking the ""Reset Password"" button, copy and paste the URL below into your web browser:</p>
                <p><a href=""{resetLink}"">{resetLink}</a></p>
            </div>
        </div>
        </body>
        </html>";
            return body;
        }

    }
}
