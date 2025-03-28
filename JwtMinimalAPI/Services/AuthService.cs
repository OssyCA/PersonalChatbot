using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Helpers;
using JwtMinimalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtMinimalAPI.Services
{
    public class AuthService(MiniJwtDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserDto request)
        {

            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                return null;
            }

            // Validate password strength
            if (!PasswordValidator.IsValid(request.Password, out var passwordErrors))
            {
                throw new ArgumentException(string.Join(", ", passwordErrors));
            }


            var user = new User(); // Create a new User

            var hashedPassword = new PasswordHasher<User>()
                    .HashPassword(user, request.Password);

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            


            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
        public async Task<TokenResponseDto?> LoginAsyc(LoginDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            TokenResponseDto response = await CreateTokenResponse(user);
            return response;
        }
        private string CreateToken(User _user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _user.UserId.ToString()), // To uniquely identify the user without relying on the Info that can be changed
                new(ClaimTypes.Name, _user.UserName),
                new(ClaimTypes.Email, _user.Email),
                new(ClaimTypes.Role, _user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("Appsettings:Token")!));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); // need 64 characters

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Appsettings:Issuer"),
                audience: configuration.GetValue<string>("Appsettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // To reduce the window of attack if the token is stolen
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        private static string RefreshTokenGenerator()
        {
            var randomNum = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNum);
            return Convert.ToBase64String(randomNum);
        }
        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = RefreshTokenGenerator();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;

        }
        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime < DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }
        public async Task<TokenResponseDto?> RefreshTokensAsync(RequestRefreshTokenDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if (user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);

        }
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto { AccessToken = CreateToken(user), RefreshToken = await GenerateAndSaveRefreshToken(user) };
        }
        public async Task<bool> RevokeRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken)  // To prevent the user from revoking another user's token
            {
                return false;
            }

            // Invalidate the refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(-1); // To invalidate the token
            await context.SaveChangesAsync();

            return true;
        } // Fix blacklisted tokens in the future
    }
}
