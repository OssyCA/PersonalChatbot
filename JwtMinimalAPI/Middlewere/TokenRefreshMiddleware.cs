using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Services;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtMinimalAPI.Middlewere
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;
        private readonly IConfiguration _configuration;


        public TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }
        // NEED FIX FOR COOKIES
        // In TokenRefreshMiddleware.cs
        public async Task InvokeAsync(HttpContext context)
        {
            // Only try to refresh if we have both cookies
            if (context.Request.Cookies.TryGetValue("accessToken", out string token) &&
                context.Request.Cookies.TryGetValue("refreshToken", out string refreshToken))
            {
                // Only refresh if token is actually near expiration or invalid
                if (!IsTokenValid(token) || IsTokenNearExpiration(token))
                {
                    var authService = context.RequestServices.GetRequiredService<IAuthService>();

                    try
                    {
                        // Extract user ID from token even if expired
                        var handler = new JwtSecurityTokenHandler();
                        var tokenWithoutValidation = handler.ReadJwtToken(token);
                        var userIdClaim = tokenWithoutValidation.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid userId))
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
                    catch
                    {
                        // Silently fail - we'll continue to the endpoint which will
                        // return 401 if token is truly invalid
                    }
                }
            }

            await _next(context);
        }
        private bool IsTokenValid(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    // These should match your token validation parameters
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Appsettings:Token"])),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Appsettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Appsettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Validate the token
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
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