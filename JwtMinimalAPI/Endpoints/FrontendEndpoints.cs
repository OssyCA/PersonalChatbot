using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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

            app.MapPost("/login", async (LoginDto request, IAuthService service) =>
            {
                var token = await service.AuthenticateUserAsync(request);
                if (token is null)
                {
                    return Results.BadRequest("Invalid username or password");
                }
                return Results.Ok(token);

            }).RequireRateLimiting("login");

            app.MapPost("/logout", async (RequestRefreshTokenDto request, IAuthService service) =>
            {
                var result = await service.RevokeRefreshTokenAsync(request.UserId, request.RefreshToken);
                if (!result)
                {
                    return Results.BadRequest("Invalid token");
                }
                return Results.Ok("Logout successful");
            }).RequireAuthorization();

            // endpoint to refresh the token
            app.MapPost("/refresh-token", async (RequestRefreshTokenDto request, IAuthService service) =>
            {
                var tokenResponse = await service.RefreshTokenPairAsync(request);
                if (tokenResponse is null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(tokenResponse);
            });

            // endpoint to test authentication
            app.MapGet("/api/auth-test", [Authorize] () => new { message = "Authentication successful" })
           .WithName("AuthTest");
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
            app.MapPut("api/changePassword", async (ChangePasswordService change, ChangePasswordDto dto) =>
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
