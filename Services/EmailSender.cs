using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AeroVista.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // fake email service to check login, baad main real mail service add kani hai
            return Task.CompletedTask;
        }
    }
}