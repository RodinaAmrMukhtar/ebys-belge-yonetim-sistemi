using Microsoft.AspNetCore.Identity.UI.Services;

namespace EBYS.Services
{
    public class DevEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // DEV MODE: do nothing
            return Task.CompletedTask;
        }
    }
}
