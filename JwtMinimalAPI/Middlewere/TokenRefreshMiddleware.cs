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
        // In TokenRefreshMiddleware.cs
        public async Task InvokeAsync(HttpContext context)
        {

            // Check if access token cookie exists
            if (context.Request.Cookies.TryGetValue("accessToken", out string token))
            {
                var authService = context.RequestServices.GetRequiredService<IAuthService>(); // Get auth service
                // Check if token is about to expire 
                if (IsTokenNearExpiration(token))
                {
                    // Get refresh token from cookie
                    if (context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    {
                        var userId = GetUserIdFromToken(token);

                        if (userId != Guid.Empty)
                        {
                            var refreshRequest = new RequestRefreshTokenDto
                            {
                                UserId = userId,
                                RefreshToken = refreshToken
                            };

                            var tokenResponse = await authService.RefreshTokenPairAsync(refreshRequest);

                            if (tokenResponse != null)
                            {
                                // Set new access token cookie
                                context.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                                });

                                // Set new refresh token cookie
                                context.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                                });
                            }
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