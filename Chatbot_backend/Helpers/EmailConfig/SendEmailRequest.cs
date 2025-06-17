namespace Chatbot_backend.Helpers.EmailConfig
{
    public record SendEmailRequest(string Recipient, string Subject, string Body);  // Record type because it's immutable and lightweight for simple data transfer
}
