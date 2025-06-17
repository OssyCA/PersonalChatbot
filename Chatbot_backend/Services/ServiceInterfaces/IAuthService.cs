using Chatbot_backend.Models;
using Chatbot_backend.DTO;

namespace Chatbot_backend.Services.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> AuthenticateUserAsync(LoginDto request);
        Task<TokenResponseDto?> RefreshTokenPairAsync(RequestRefreshTokenDto request);
        Task<bool> RevokeRefreshTokenAsync(Guid userId);
        string CreateToken(User user);
        string GenerateRefreshToken();
    }
}
