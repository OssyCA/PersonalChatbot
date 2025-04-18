using JwtMinimalAPI.Helpers.EmailConfig;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using JwtMinimalAPI.Services.ServiceInterfaces;

namespace JwtMinimalAPI.Services
{
    public class MailService(IOptions<GmailOptions> gmailOptions) : IMailService
    {
        private readonly GmailOptions _gmailOptions = gmailOptions.Value; // Gmail SMTP server configuration
        public async Task<bool> SendEmailAsync(SendEmailRequest sendEmailRequest)
        {
            try // Try to send an email
            {
                MailMessage mail = new()
                {
                    From = new MailAddress(_gmailOptions.Email),
                    Subject = sendEmailRequest.Subject,
                    Body = sendEmailRequest.Body,
                };
                mail.To.Add(sendEmailRequest.Recipient);

                // Configure the SMTP client
                using var smtpClient = new SmtpClient();
                smtpClient.Host = _gmailOptions.Host;
                smtpClient.Port = _gmailOptions.Port;
                smtpClient.Credentials = new NetworkCredential(
                    _gmailOptions.Email, _gmailOptions.Password);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

