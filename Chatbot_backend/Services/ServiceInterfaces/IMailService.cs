using Chatbot_backend.Helpers.EmailConfig;

namespace Chatbot_backend.Services.ServiceInterfaces
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(SendEmailRequest sendEmailRequest);
    }
}
