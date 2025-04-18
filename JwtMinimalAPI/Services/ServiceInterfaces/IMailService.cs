using JwtMinimalAPI.Helpers.EmailConfig;

namespace JwtMinimalAPI.Services.ServiceInterfaces
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(SendEmailRequest sendEmailRequest);
    }
}
