using JwtMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtMinimalAPI.Data
{
    public class MiniJwtDbContext(DbContextOptions<MiniJwtDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
