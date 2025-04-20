
using Azure.Core;
using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Services
{
    public class PasswordService(MiniJwtDbContext context)
    {
        public async Task<bool> ChangedPassword(ChangePasswordDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user == null)
            {
                // User not found
                return false;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword)
                == PasswordVerificationResult.Failed)
            {
                // Old password is incorrect
                return false;
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
            var user = context.Users.FirstOrDefault(u => u.Email == dto.Email); // Find user by email

            // Check if the user exists
            if (user == null)  
            {
                return false;
            }
            // Validate password strength
            if (!PasswordValidator.IsValid(dto.NewPassword, out var passwordErrors))
            {
                throw new ArgumentException(string.Join(", ", passwordErrors));
            }


            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, dto.NewPassword); // Hash the new password

            return true;

        }
    }
}
