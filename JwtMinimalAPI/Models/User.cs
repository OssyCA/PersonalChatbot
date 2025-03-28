using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JwtMinimalAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }

    }
}
