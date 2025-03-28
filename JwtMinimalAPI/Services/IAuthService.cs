using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;

namespace JwtMinimalAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsyc(LoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RequestRefreshTokenDto request);
        Task<bool> RevokeRefreshTokenAsync(Guid userId, string refreshToken);
    }
}
