using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtMinimalAPI.Endpoints
{
    public class FrontendEndpoints()
    {
        public static void HandleUser(WebApplication app)
        {
            app.MapPost("/register", async (UserDto request, IAuthService service, IMailService mailService) =>
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


            });

            // In the login endpoint
            app.MapPost("/login", async (LoginDto request, IAuthService service, HttpContext httpContext) =>
            {
                var tokenResponse = await service.AuthenticateUserAsync(request);
                if (tokenResponse is null)
                {
                    return Results.BadRequest("Invalid username or password");
                }

                // Set access token in HTTP-only cookie
                httpContext.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Use in production with HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15) // Match token expiration
                });

                // Set refresh token in HTTP-only cookie
                httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Use in production with HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // Match token expiration
                });

                // Send user info that's needed on the client side
                var payload = GetUserInfoFromToken(tokenResponse.AccessToken);
                return Results.Ok(new
                {
                    userId = payload.UserId,
                    username = payload.Username,
                    email = payload.Email
                });

            }).RequireRateLimiting("login");



            app.MapPost("/logout", async (IAuthService service, HttpContext httpContext) =>
            {
                // Get user ID and refresh token from cookies
                var refreshToken = httpContext.Request.Cookies["refreshToken"];
                // Get user ID from the token claim
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null && refreshToken != null)
                {
                    var result = await service.RevokeRefreshTokenAsync(Guid.Parse(userId), refreshToken);
                }

                // Clear cookies regardless of revoke result
                httpContext.Response.Cookies.Delete("accessToken");
                httpContext.Response.Cookies.Delete("refreshToken");

                return Results.Ok("Logout successful");
            }).RequireAuthorization();

            // endpoint to refresh the token
            // Update refresh token endpoint
            app.MapPost("/refresh-token", async (HttpContext httpContext, IAuthService service) =>
            {
                var refreshToken = httpContext.Request.Cookies["refreshToken"];
                // For user ID, we could either get it from the cookie or from a claim if authenticated
                // For this example, we'll get it from the user claim if available
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(refreshToken))
                {
                    return Results.Unauthorized();
                }

                var request = new RequestRefreshTokenDto
                {
                    UserId = Guid.Parse(userId),
                    RefreshToken = refreshToken
                };

                var tokenResponse = await service.RefreshTokenPairAsync(request);
                if (tokenResponse is null)
                {
                    return Results.Unauthorized();
                }

                // Set new access token
                httpContext.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                // Set new refresh token
                httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Results.Ok(new { success = true });
            });

            // endpoint to test authentication
            app.MapGet("/api/auth-test", [Authorize] () => new { message = "Authentication successful" })
           .WithName("AuthTest");
        }
        private static dynamic GetUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            return new
            {
                UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };
        }
        public static void HandleChatBot(WebApplication app)
        {
            app.MapPost("/InputMessage/{inputMessage}", async (ChatBotService service, string inputMessage) =>
            {
                var chatResponse = await service.GetResponse(inputMessage);
                return Results.Ok(chatResponse);
            }).RequireAuthorization();
        }
        public static void ChangePassword(WebApplication app)
        {
            app.MapPut("change-password", async (ChangePasswordService change, ChangePasswordDto dto) =>
            {
                var changedPassword = await change.ChangedPassword(dto);

                if (changedPassword)
                {
                    return Results.Ok("Passwordchanged");
                }
                else
                {
                    return Results.BadRequest();
                }
            });
        }
    }
}
