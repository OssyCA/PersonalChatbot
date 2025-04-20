
using Azure.Core;
using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Services
{
    public class PasswordService(MiniJwtDbContext context, ILogger<PasswordService> _logger)
    {
        public async Task<bool> ChangedPassword(ChangePasswordDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user == null)
            {
                // User not found
                throw new KeyNotFoundException();
            }

            // Validate password strength
            if (!PasswordValidator.IsValid(dto.NewPassword, out var passwordErrors))
            {
                throw new ArgumentException(string.Join(", ", passwordErrors));
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword)
                == PasswordVerificationResult.Failed)
            {
                // Old password is incorrect
                throw new UnauthorizedAccessException();
            }
            else
            {
                // Hash the new password
                var hashedNew = new PasswordHasher<User>()
                    .HashPassword(user, dto.NewPassword);

                user.PasswordHash = hashedNew;

                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> ResetPassword(ResetPasswordDto dto)
        {
            _logger.LogInformation("ResetPassword called with email: {Email} and token: {Token}", dto.Email, dto.ForgotPasswordToken);

            var user = await context.Users.FirstOrDefaultAsync(u =>
                u.Email == dto.Email &&
                u.ForgotPasswordToken == dto.ForgotPasswordToken &&
                u.ForgotPasswordTokenExpireTime > DateTime.UtcNow);

            if (user == null)
            {
                return false;
            }

            // Validate password strength
            if (!PasswordValidator.IsValid(dto.NewPassword, out var passwordErrors))
            {
                throw new ArgumentException(string.Join(", ", passwordErrors));
            }

            // Hash the new password
            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, dto.NewPassword);

            // Clear the reset token
            user.ForgotPasswordToken = null;
            user.ForgotPasswordTokenExpireTime = null;

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ForgotPassword(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return false; // User not found
            }

            // Generate token with expiration
            user.ForgotPasswordToken = Guid.NewGuid();
            user.ForgotPasswordTokenExpireTime = DateTime.UtcNow.AddHours(1);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
