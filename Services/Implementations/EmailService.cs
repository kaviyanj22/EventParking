using System.Net;
using System.Net.Mail;
using Event_parking.Configurations;
using Event_parking.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Event_parking.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(
            IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody)
        {
            using var emailMessage = new MailMessage();

            emailMessage.From = new MailAddress(
                _emailSettings.SenderEmail,
                _emailSettings.SenderName
            );

            emailMessage.To.Add(recipientEmail);

            emailMessage.Subject = subject;

            emailMessage.Body = htmlBody;

            emailMessage.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(
                _emailSettings.SmtpServer,
                _emailSettings.Port
            );

            smtpClient.EnableSsl = true;

            smtpClient.Credentials =
                new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password
                );

            await smtpClient.SendMailAsync(emailMessage);
        }
    }
}
