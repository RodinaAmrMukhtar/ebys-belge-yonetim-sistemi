using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using EBYS.Models;

namespace EBYS.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient(_settings.Smtp, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Sender,
                    _settings.Password
                ),
                EnableSsl = true
            };

            var mail = new MailMessage(
                _settings.Sender,
                to,
                subject,
                body
            );

            await smtp.SendMailAsync(mail);
        }
    }
}
