
using Chatbot_backend.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Chatbot_backend.Helpers.GetUserIdHelper
{
    public class GetUserIdFromToken
    {
        public static Guid TryGetUserIdFromToken(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Guid.Empty;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!tokenHandler.CanReadToken(token))
                {
                    return Guid.Empty;
                }
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                {
                    return Guid.Empty;
                }
                return userId;
            }
            catch
            {
                // Token may be invalid/expired - silently handle the error
                return Guid.Empty;
            }
        }
        public static async Task<Guid> GetValidatedUserIdAsync(HttpContext httpContext, MiniJwtDbContext context)
        {
            // First try to get userId from token
            Guid userId = TryGetUserIdFromToken(httpContext.Request.Cookies["accessToken"]);

            // If that fails (token expired or invalid), try to get userId from cookies + validate
            if (userId == Guid.Empty &&
                httpContext.Request.Cookies.TryGetValue("userId", out string? userIdString) &&
                Guid.TryParse(userIdString, out Guid cookieUserId) &&
                cookieUserId != Guid.Empty &&
                httpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken) &&
                !string.IsNullOrEmpty(refreshToken))
            {
                // Verify user exists in database with valid refresh token
                var user = await context.Users.FirstOrDefaultAsync(u =>
                    u.UserId == cookieUserId &&
                    u.RefreshToken == refreshToken &&
                    u.RefreshTokenExpireTime > DateTime.UtcNow);

                if (user != null)
                {
                    userId = user.UserId;
                }
            }

            return userId;
        }
    }
}
