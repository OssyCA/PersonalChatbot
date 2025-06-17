namespace Chatbot_backend.Helpers.EmailConfig
{
    public class GmailOptions
    {
        public const string GmailOptionsKey = "GmailOptions"; // Key in appsettings.json
        // Properties to store the Gmail SMTP server configuration
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
