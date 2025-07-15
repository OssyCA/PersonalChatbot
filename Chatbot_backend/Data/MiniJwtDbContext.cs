using Chatbot_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Data
{
    public class MiniJwtDbContext(DbContextOptions<MiniJwtDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        //public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserAId, f.UserBId });

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserA)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserAId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserB)
                .WithMany()
                .HasForeignKey(f => f.UserBId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
