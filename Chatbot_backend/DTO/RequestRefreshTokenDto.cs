namespace Chatbot_backend.DTO
{
    public class RequestRefreshTokenDto
    {
        public Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
