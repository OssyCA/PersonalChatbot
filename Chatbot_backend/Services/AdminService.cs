using Chatbot_backend.Data;
using Chatbot_backend.DTO;
using Chatbot_backend.Models;
using Chatbot_backend.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Services
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
