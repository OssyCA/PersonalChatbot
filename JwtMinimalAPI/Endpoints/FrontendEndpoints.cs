using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Middlewere;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
            app.MapPost("/login", async (LoginDto request, IAuthService service, HttpContext httpContext) =>
            {
                var tokenResponse = await service.AuthenticateUserAsync(request);
                if (tokenResponse is null)
                {
                    return Results.BadRequest("Invalid username or password");
                }

                // Get the user info
                var user = await service.GetUserByUsernameAsync(request.UserName);
                if (user == null)
                {
                    return Results.BadRequest("User not found");
                }

                // Set cookies
                httpContext.Response.Cookies.Append("accessToken", tokenResponse.AccessToken, GetCookieOptionsData.AccessTokenCookie());

                httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, GetCookieOptionsData.RefreshTokenCookie());

                //Return user info(excluding password)
                return Results.Ok(new
                {
                    userId = user.UserId.ToString(),
                    username = user.UserName,
                    email = user.Email
                });

                //return Results.Ok(tokenResponse); // use to return the token response in scalar api test
            });
            app.MapPost("/logout", (HttpContext httpContext) =>
            {
                // Clear cookies
                httpContext.Response.Cookies.Delete("accessToken", GetCookieOptionsData.AccessTokenCookie());
                httpContext.Response.Cookies.Delete("refreshToken", GetCookieOptionsData.RefreshTokenCookie());

                return Results.Ok("Logged out successfully");
            });
            app.MapPost("/refresh-token", async (HttpContext httpContext, IAuthService service) =>
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
                    httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, GetCookieOptionsData.RefreshTokenCookie());

                    return Results.Ok(new { success = true });
                }
                catch
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous();
            // endpoint to test authentication
            app.MapGet("/api/auth-test", [Authorize] () => new { message = "Authentication successful" })
           .WithName("AuthTest");
            app.MapGet("/api/public-test", () => Results.Ok(new { message = "This is a public endpoint" }));
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
            app.MapPost("change-password", async (ChangePasswordService change, ChangePasswordDto dto) =>
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
        public static void AdminManage(WebApplication app)
        {
            app.MapGet("api/GetUsers", async (IAdminService service) =>
            {
                var user = await service.GetUsers();

                if (user is not null)
                {
                    return Results.Ok(user);
                }
                return Results.NotFound("No users");


            }).RequireAuthorization("AdminPolicy");
        }
    }
}
