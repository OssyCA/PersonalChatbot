﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Chatbot_backend.Models
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
        public Guid? ForgotPasswordToken { get; set; }
        public DateTime? ForgotPasswordTokenExpireTime { get; set; }

    }
}
