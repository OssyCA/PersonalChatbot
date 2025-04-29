using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers.EmailConfig;
using JwtMinimalAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtMinimalAPI.Services.ServiceInterfaces;
using JwtMinimalAPI.Services;

namespace JwtMinimalAPI.Endpoints
{
    public static class UserEndpoints
    {
        // FIX LOGOUT AND GETUSERMETOD, ADD USERID TO COOKIES
        public static void GetUserEndpoints(WebApplication app)
        {
            app.MapPost("/register", RegisterUser);
            app.MapPost("/login", LoginUser).RequireRateLimiting("login");
            app.MapPost("/logout", LogoutUser);
            app.MapPost("/refresh-token", RefreshToken).AllowAnonymous();
           

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
        private static async Task<IResult> LoginUser(LoginDto request, IAuthService service, HttpContext httpContext)
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
        }
        private static IResult LogoutUser(HttpContext httpContext)
        {
            // REVOKE REFRESH TOKEN LATER

            // Clear cookies
            httpContext.Response.Cookies.Delete("accessToken", GetCookieOptionsData.AccessTokenCookie());
            httpContext.Response.Cookies.Delete("refreshToken", GetCookieOptionsData.RefreshTokenCookie());

            return Results.Ok("Logged out successfully");
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
                httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, GetCookieOptionsData.RefreshTokenCookie());

                return Results.Ok(new { success = true });
            }
            catch
            {
                return Results.Unauthorized();
            }
        }
       
    }
}
