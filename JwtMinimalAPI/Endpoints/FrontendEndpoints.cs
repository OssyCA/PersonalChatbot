using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Services;

namespace JwtMinimalAPI.Endpoints
{
    public class FrontendEndpoints(IConfiguration configuration)
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
                //var validateErrors = ValidateObjects.ValidateObject(request); // to provide validation for the request 
                //if (validateErrors.Count > 0)
                //{
                //    return Results.BadRequest(new { errors = validateErrors });
                //}

                var token = await service.LoginAsyc(request);
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
                var tokenResponse = await service.RefreshTokensAsync(request);
                if (tokenResponse is null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(tokenResponse);
            });
        }

        public static void HandleChatBot(WebApplication app)
        {
            app.MapPost("/InputMessage/{inputMessage}", async (ChatBotService service, string inputMessage) =>
            {
                var chatResponse = await service.GetResponse(inputMessage);
                return Results.Ok(chatResponse);
            }).RequireAuthorization();
        }
    }
}
