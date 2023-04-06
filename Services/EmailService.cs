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
             _mailSettings = mailSettings.Value;
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.MailAddress ?? Environment.GetEnvironmentVariable("MailAddress");
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSender);
            email.To.Add(MailboxAddress.Parse(emailSender));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort == 0 ? int.Parse(Environment.GetEnvironmentVariable("MailPort")) : _mailSettings.MailPort;
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");
                smtp.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailSender, password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch(Exception ex)
            {
                var error = ex.Message;
                throw;
            }
            
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.MailAddress ?? Environment.GetEnvironmentVariable("MailAddress");
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSender);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort == 0 ? int.Parse(Environment.GetEnvironmentVariable("MailPort")) : _mailSettings.MailPort;
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");
                smtp.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailSender, password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}
