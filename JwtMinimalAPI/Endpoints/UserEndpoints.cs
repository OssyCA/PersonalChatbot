using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtMinimalAPI.Services.ServiceInterfaces;
using JwtMinimalAPI.Services;
using JwtMinimalAPI.Helpers.GetUserIdHelper;
using Microsoft.EntityFrameworkCore;
using Stripe;
using JwtMinimalAPI.Data;
using System.Threading.Tasks;

namespace JwtMinimalAPI.Endpoints
{
    public class UserEndpoints
    {
        // FIX LOGOUT AND GETUSERMETOD, ADD USERID TO COOKIES
        private static ILogger<UserEndpoints> _logger;
        public static void GetUserEndpoints(WebApplication app)
        {
            _logger = app.Services.GetRequiredService<ILogger<UserEndpoints>>();


            app.MapPost("/api/register", RegisterUser);
            app.MapPost("/api/login", LoginUser).RequireRateLimiting("login");
            app.MapPost("/api/logout", LogoutUser);
            app.MapPost("/api/refresh-token", RefreshToken).AllowAnonymous();
            

            // Test endpoints
            app.MapGet("/api/auth-test", [Authorize] () => new { message = "Authentication successful" })
                .WithName("AuthTest");
            app.MapGet("/api/public-test", () => Results.Ok(new { message = "This is a public endpoint" }));
        }
        // IResult let us return different types of results, like Ok, BadRequest, Unauthorized, etc.
        private static async Task<IResult> RegisterUser(UserDto request, IAuthService service, IMailService mailService)
        {
            var validateErrors = ValidateObjects.ValidateObject(request);

            if (validateErrors.Count > 0)
            {
                return Results.BadRequest(new { errors = validateErrors });
            }

            var user = await service.RegisterAsync(request);

            if (user is null)
            {
                return Results.BadRequest("Username Already Exist");
            }

            // Send email to the user after registration 
            SendEmailRequest email = new(
                user.Email,
                "Welcome to the JwtMinimalAPI",
                $"You have successfully registered as {user.UserName}"
            );

            await mailService.SendEmailAsync(email);

            return Results.Ok(user);
        }
        private static async Task<IResult> LoginUser(LoginDto request, IAuthService service, HttpContext httpContext, MiniJwtDbContext context)
        {
            if (request == null ||
                    string.IsNullOrWhiteSpace(request.UserName) ||
                    string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Invalid login attempt");
                return Results.BadRequest(new { error = "Email and password are required" });
            }
            var tokenResponse = await service.AuthenticateUserAsync(request);
            if (tokenResponse is null)
            {
                return Results.BadRequest("Invalid username or password");
            }

            // Get the user info
            var userId = GetUserIdFromToken.TryGetUserIdFromToken(tokenResponse.AccessToken);
            if (userId == Guid.Empty)
            {
                return Results.BadRequest("Invalid token");
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            // Set cookies
            SetAuthenticationCookies(httpContext, tokenResponse.AccessToken, tokenResponse.RefreshToken, context);

            //Return user info(excluding password) REMOVE LATER
            return Results.Ok(new
            {
                userId = user.UserId.ToString(),
                username = user.UserName,
                email = user.Email
            });
        }
        private static async Task<IResult> LogoutUser(HttpContext httpContext, MiniJwtDbContext context, IAuthService service)
        {
            // REVOKE REFRESH TOKEN LATER
            try
            {
                var userId = await GetUserIdFromToken.GetValidatedUserIdAsync(httpContext, context);


                // Delete all authentication cookies
                httpContext.Response.Cookies.Delete("accessToken", GetCookieOptionsData.AccessTokenCookie());
                httpContext.Response.Cookies.Delete("refreshToken", GetCookieOptionsData.RefreshTokenAndUserIdCookie());
                httpContext.Response.Cookies.Delete("userId", GetCookieOptionsData.RefreshTokenAndUserIdCookie());

                // Revoke tokens in database if user identified
                if (userId > Guid.Empty)
                {
                    await service.RevokeRefreshTokenAsync(userId);
                    return Results.Ok(new { message = "Logged out successfully" });
                }
                // if userId is still 0, it means the user was not found or the token was invalid
                return Results.Ok(new { message = "Session cleared" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout: {Message}", ex.Message);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        private static async Task<IResult> RefreshToken(HttpContext httpContext, IAuthService service)
        {
            // Check if refresh token exists in cookies
            if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
            {
                return Results.Unauthorized();
            }

            // Extract user ID from the access token (even if it's expired)
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var accessToken = httpContext.Request.Cookies["accessToken"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    return Results.Unauthorized();
                }

                var tokenWithoutValidation = tokenHandler.ReadJwtToken(accessToken); // Read the token without validation
                var userIdClaim = tokenWithoutValidation.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Results.Unauthorized();
                }

                // Create the request object to refresh the token pair
                var refreshRequest = new RequestRefreshTokenDto
                {
                    UserId = userId,
                    RefreshToken = refreshToken
                };

                // Use the service method
                var tokenResponse = await service.RefreshTokenPairAsync(refreshRequest);

                if (tokenResponse == null)
                {
                    return Results.Unauthorized();
                }

                // Set new cookies
                httpContext.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, GetCookieOptionsData.AccessTokenCookie());
                httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, GetCookieOptionsData.RefreshTokenAndUserIdCookie());

                return Results.Ok(new { success = true });
            }
            catch
            {
                return Results.Unauthorized();
            }
        }
        private static void SetAuthenticationCookies(HttpContext httpContext, string accessToken, string refreshToken, MiniJwtDbContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    throw new ArgumentException("Tokens cannot be null or empty");
                }

                // Extract user ID from the token
                var userId = GetUserIdFromToken.GetValidatedUserIdAsync(httpContext, context);
                httpContext.Response.Cookies.Append("userId", userId.ToString(), GetCookieOptionsData.RefreshTokenAndUserIdCookie());

                httpContext.Response.Cookies.Append("accessToken", accessToken, GetCookieOptionsData.AccessTokenCookie());
                httpContext.Response.Cookies.Append("refreshToken", refreshToken, GetCookieOptionsData.RefreshTokenAndUserIdCookie());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set authentication cookies: {Message}", ex.Message);
                throw;
            }
        }
    }

}

