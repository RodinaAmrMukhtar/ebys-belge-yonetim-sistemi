using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace EBYS.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // DO NOTHING – PREVENTS IDENTITY ERRORS
            return Task.CompletedTask;
        }
    }
}
