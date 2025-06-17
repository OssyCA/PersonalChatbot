using Chatbot_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Data
{
    public class MiniJwtDbContext(DbContextOptions<MiniJwtDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
