using Chatbot_backend.DTO;
using Chatbot_backend.Helpers;
using Chatbot_backend.Services.ServiceInterfaces;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatbot_backend.Middlewere
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

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogWarning("Processing request in TokenRefreshMiddleware");

            // Only try to refresh if we have both cookies
            if (context.Request.Cookies.TryGetValue("accessToken", out string? token) &&
                context.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
            {
                _logger.LogDebug("Found both access and refresh tokens in cookies");

                // Only refresh if token is actually near expiration or invalid
                bool isValid = IsTokenValid(token);
                bool isNearExpiration = IsTokenNearExpiration(token);

                if (!isValid)
                {
                    _logger.LogInformation("Access token is invalid, attempting refresh");
                }
                else if (isNearExpiration)
                {
                    _logger.LogInformation("Access token is near expiration, attempting refresh");
                }

                if (!isValid || isNearExpiration)
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
                            _logger.LogDebug("Successfully extracted user ID {UserId} from token", userId);

                            var refreshRequest = new RequestRefreshTokenDto
                            {
                                UserId = userId,
                                RefreshToken = refreshToken
                            };

                            _logger.LogDebug("Attempting to refresh token for user {UserId}", userId);
                            var tokenResponse = await authService.RefreshTokenPairAsync(refreshRequest);

                            if (tokenResponse != null)
                            {
                                _logger.LogInformation("Successfully refreshed tokens for user {UserId}", userId);

                                // Set new access token cookie
                                context.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, GetCookieOptionsData.AccessTokenCookie());

                                // Set new refresh token cookie
                                context.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, GetCookieOptionsData.RefreshTokenAndUserIdCookie());
                            }
                            else
                            {
                                _logger.LogWarning("Token refresh returned null for user {UserId}", userId);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Failed to extract valid user ID from token");
                        }
                    }
                    catch (SecurityTokenException tokenEx)
                    {
                        _logger.LogWarning("Token security validation failed during refresh: {Message}", tokenEx.Message);
                    }
                    catch (InvalidOperationException opEx)
                    {
                        _logger.LogWarning("Invalid operation during token refresh: {Message}", opEx.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error during token refresh attempt");
                        // Optionally clear cookies on critical errors but better safty
                        context.Response.Cookies.Delete("accessToken");
                        context.Response.Cookies.Delete("refreshToken");
                    }
                }
            }
            else
            {
                _logger.LogDebug("One or both tokens missing from cookies - skipping refresh");
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
            catch (Exception ex)
            {
                _logger.LogDebug("Token validation failed: {Message}", ex.Message);
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
                {
                    _logger.LogDebug("Could not parse token as JwtSecurityToken");
                    return true;
                }

                // Check if token expires in less than set time
                var expirationTime = jwtToken.ValidTo.ToUniversalTime();
                var timeUntilExpiration = expirationTime - DateTime.UtcNow;
                var isNearExpiration = expirationTime < DateTime.UtcNow.AddMinutes(5);

                if (isNearExpiration)
                {
                    _logger.LogDebug("Token expires in {MinutesRemaining} minutes", timeUntilExpiration.TotalMinutes);
                }

                return isNearExpiration;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error checking token expiration: {Message}", ex.Message);
                // If there's an error reading the token, consider it expired
                return true;
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