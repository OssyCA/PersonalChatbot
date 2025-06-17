namespace Chatbot_backend.DTO
{
    public class ChangePasswordDto
    {
        public string UserName { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
