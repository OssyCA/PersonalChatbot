using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JwtMinimalAPI.Endpoints
{
    public class TestingPoints(IConfiguration configuration)
    {
        public static void HandleUsers(WebApplication app)
        {
            string endpoinsGroup = "logIn";

            app.MapPost("/register", async (UserDto request, IAuthService service) =>
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

                return Results.Ok(user);


            }).WithTags(endpoinsGroup);

            app.MapPost("/login", async (LoginDto request, IAuthService service) =>
            {
                var token = await service.AuthenticateUserAsync(request);
                if (token is null)
                {
                    return Results.BadRequest("Invalid username or password");
                }
                return Results.Ok(token);

            }).WithTags(endpoinsGroup);

            app.MapGet("/loginAuth", async () =>
            {
                return Results.Ok("You are authenticated");
            }).WithTags("AUTH ONLY").RequireAuthorization();

            app.MapGet("/loginAdminOnly", () =>
            {
                return Results.Ok("Admin login");
            }).WithTags("AUTH ONLY").RequireAuthorization("AdminPolicy");
            app.MapPost("/refrshToken", async (RequestRefreshTokenDto request, IAuthService service) =>
            {
                var result = await service.RefreshTokenPairAsync(request);
                if (result is null || result.AccessToken is null || request.RefreshToken is null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(result);

            });

            app.MapPost("/email", async (SendEmailRequest request, IMailService service) =>
            {

                var result = await service.SendEmailAsync(request);

                if (!result)
                {
                    return Results.BadRequest("Email not sent");
                }
                return Results.Ok("Email sent");
            });
        }
    }
}
