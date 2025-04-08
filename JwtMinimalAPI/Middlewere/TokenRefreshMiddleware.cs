using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Services;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtMinimalAPI.Middlewere
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;

        public TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        // NEED FIX FOR COOKIES
        public async Task InvokeAsync(HttpContext context)
        {

            // Get service during request insted of at startup
            var authService = context.RequestServices.GetRequiredService<IAuthService>();

            // Check if Authorization header is present
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) &&
                authHeader.ToString().StartsWith("Bearer "))
            {
                var token = authHeader.ToString().Substring("Bearer ".Length).Trim();

                // Check if token is about to expire 
                if (IsTokenNearExpiration(token))
                {
                    _logger.LogInformation("Token is near expiration, attempting to refresh");

                    // Get user ID from token claims
                    var userId = GetUserIdFromToken(token);

                    // Get refresh token from custom header
                    if (context.Request.Headers.TryGetValue("X-Refresh-Token", out var refreshToken) && userId != Guid.Empty)
                    {
                        _logger.LogInformation($"Refresh token found in headers. User ID: {userId}");

                        var refreshRequest = new RequestRefreshTokenDto
                        {
                            UserId = userId,
                            RefreshToken = refreshToken
                        };

                        _logger.LogInformation("Calling RefreshTokenPairAsync...");
                        var tokenResponse = await authService.RefreshTokenPairAsync(refreshRequest);

                        if (tokenResponse != null)
                        {
                            // Set the new access token in the request header
                            context.Request.Headers["Authorization"] = $"Bearer {tokenResponse.AccessToken}";

                            // Add new tokens to response headers
                            _logger.LogInformation("Tokens refreshed successfully");
                            context.Response.Headers.Append("X-Access-Token", tokenResponse.AccessToken);
                            context.Response.Headers.Append("X-Refresh-Token", tokenResponse.RefreshToken);
                        }
                        else
                        {
                            _logger.LogWarning("Token refresh failed - RefreshTokenPairAsync returned null");
                        }
                    }
                    else
                    {
                        if (!context.Request.Headers.TryGetValue("X-Refresh-Token", out _))
                        {
                            _logger.LogWarning("X-Refresh-Token header not found in request");
                        }

                        if (userId == Guid.Empty)
                        {
                            _logger.LogWarning("User ID from token is empty or invalid");
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

                // Check if token expires in less than set time
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

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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

    // Extension method for registration in Program.cs
    public static class TokenRefreshMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRefreshMiddleware>();
        }
    }
}