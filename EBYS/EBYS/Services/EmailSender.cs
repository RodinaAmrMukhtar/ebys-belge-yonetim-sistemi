using Microsoft.AspNetCore.Identity.UI.Services;

namespace EBYS.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TEMP: just log, no real email
            Console.WriteLine($"Email TO: {email}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine(htmlMessage);

            return Task.CompletedTask;
        }
    }
}
