using Chatbot_backend.Data;
using Chatbot_backend.DTO;
using Chatbot_backend.Helpers;
using Chatbot_backend.Helpers.EmailConfig;
using Chatbot_backend.Services;
using Chatbot_backend.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Endpoints
{
    public static class PasswordEndpoints
    {
        public static void GetPasswordEndpoints(WebApplication app)
        {
            app.MapPost("/change-password", ChangeUserPassword);
            app.MapPost("/api/forgot-password", ForgotPassword).AllowAnonymous();
            app.MapPost("/api/reset-password", ResetPassword).AllowAnonymous();
        }
        private static async Task<IResult> ChangeUserPassword(PasswordService change, ChangePasswordDto dto)
        {
            var changedPassword = await change.ChangedPassword(dto);

            if (changedPassword)
            {
                return Results.Ok("Password changed");
            }
            else
            {
                return Results.BadRequest();
            }
        }
        private static async Task<IResult> ForgotPassword(PasswordService service, IMailService mailService, ForgotPasswordDto dto, MiniJwtDbContext context)
        {
            var result = await service.ForgotPassword(dto.Email);

            if (result)
            {
                // Get the user to get token information
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user != null && user.ForgotPasswordToken.HasValue && user.ForgotPasswordTokenExpireTime.HasValue)
                {
                    // Format expiry as ISO string which is safe for URLs
                    var expiryString = user.ForgotPasswordTokenExpireTime.Value.ToString("o");

                    // Send email with reset link
                    var resetLink = $"http://localhost:5173/resetpassword?token={user.ForgotPasswordToken}&email={user.Email}&expiry={Uri.EscapeDataString(expiryString)}";

                    var emailRequest = new SendEmailRequest(
                        dto.Email,
                        "Password Reset Request",
                        $"Please click the following link to reset your password: {resetLink}"
                    );

                    await mailService.SendEmailAsync(emailRequest);
                }
            }

            // For security, always return the same message
            return Results.Ok("If your email is registered in our system, you will receive a password reset link.");
        }
        private static async Task<IResult> ResetPassword(ResetPasswordDto dto, PasswordService passwordService)
        {

            var validateErrors = ValidateObjects.ValidateObject(dto);
            if (validateErrors.Count > 0)
            {
                return Results.BadRequest(new { errors = validateErrors });
            }

            var result = await passwordService.ResetPassword(dto);

            if (result)
            {
                return Results.Ok("Password reset successfully");
            }
            else
            {
                return Results.BadRequest("Failed to reset password");
            }
        }
    }
}
