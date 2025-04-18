using JwtMinimalAPI.Data;
using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;
using JwtMinimalAPI.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Services
{
    public class AdminService(MiniJwtDbContext context) : IAdminService
    {
        public async Task<List<ShowUserDto>> GetUsers()
        {
            var allUsers = await context.Users.Select(u => new ShowUserDto
            {
                UserId = u.UserId,
                Username = u.UserName,
                Email = u.Email

            }).ToListAsync();

            return allUsers;
        }
    }
}
