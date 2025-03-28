using JwtMinimalAPI.Helpers.EmailConfig;

namespace JwtMinimalAPI.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(SendEmailRequest sendEmailRequest);
    }
}
