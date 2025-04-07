// JwtMinimalAPI/Middlewere/TokenRefreshMiddleware.cs
using JwtMinimalAPI.Services;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace JwtMinimalAPI.Middlewere
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authService;
        private readonly ILogger<TokenRefreshMiddleware> _logger;

        public TokenRefreshMiddleware(RequestDelegate next, IAuthService authService, ILogger<TokenRefreshMiddleware> logger)
        {
            _next = next;
            _authService = authService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if Authorization header is present
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) &&
                authHeader.ToString().StartsWith("Bearer "))
            {
                var token = authHeader.ToString().Substring("Bearer ".Length).Trim();

                // Check if token is about to expire (within 5 minutes)
                if (IsTokenNearExpiration(token))
                {
                    _logger.LogInformation("Token is near expiration, attempting to refresh");

                    // Get user ID from token claims
                    var userId = GetUserIdFromToken(token);

                    // Get refresh token from request cookies
                    if (context.Request.Cookies.TryGetValue("refreshToken", out string refreshToken) && userId != Guid.Empty)
                    {
                        var refreshRequest = new JwtMinimalAPI.DTO.RequestRefreshTokenDto
                        {
                            UserId = userId,
                            RefreshToken = refreshToken
                        };

                        var tokenResponse = await _authService.RefreshTokensAsync(refreshRequest);

                        if (tokenResponse != null)
                        {
                            // Set the new access token in the request header
                            context.Request.Headers["Authorization"] = $"Bearer {tokenResponse.AccessToken}";

                            // Set new refresh token cookie
                            context.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTime.UtcNow.AddDays(7)
                            });

                            // Also add them to response headers so frontend can update localStorage
                            context.Response.Headers.Append("X-Access-Token", tokenResponse.AccessToken);
                            context.Response.Headers.Append("X-Refresh-Token", tokenResponse.RefreshToken);
                        }
                    }
                }
            }

            await _next(context);
        }

        private bool IsTokenNearExpiration(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return true;

                // Check if token expires in less than 5 minutes
                return jwtToken.ValidTo.ToUniversalTime() < DateTime.UtcNow.AddMinutes(5);
            }
            catch
            {
                // If there's an error reading the token, consider it expired
                return true;
            }
        }

        private Guid GetUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return Guid.Empty;

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                if (userIdClaim != null && Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }

                return Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }

    // Extension method for easy registration in Program.cs
    public static class TokenRefreshMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRefreshMiddleware>();
        }
    }
}