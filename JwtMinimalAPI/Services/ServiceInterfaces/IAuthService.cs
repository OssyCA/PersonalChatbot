using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;

namespace JwtMinimalAPI.Services.ServiceInterfaces
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
