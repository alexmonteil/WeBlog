using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using WeBlog.Configuration;
using WeBlog.Services.Interfaces;

namespace WeBlog.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            if (mailSettings != null && mailSettings.Value != null)
            {
                _mailSettings = mailSettings.Value;
            }
            else
            {
                _mailSettings = new MailSettings
                {
                    MailAddress = Environment.GetEnvironmentVariable("MailAddress"),
                    DisplayName = Environment.GetEnvironmentVariable("DisplayName"),
                    MailPassword = Environment.GetEnvironmentVariable("MailPassword"),
                    MailHost = Environment.GetEnvironmentVariable("MailHost"),
                    MailPort = int.Parse(Environment.GetEnvironmentVariable("MailPort"))
                };
            }
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.MailAddress);
            email.To.Add(MailboxAddress.Parse(_mailSettings.MailAddress));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.MailAddress, _mailSettings.MailPassword);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.MailAddress);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.MailAddress, _mailSettings.MailPassword);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
