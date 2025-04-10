﻿using Azure.Core;
using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Services
{
    public class ChangePasswordService(MiniJwtDbContext context)
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
    }
}
