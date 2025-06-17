using System.ComponentModel.DataAnnotations;

namespace Chatbot_backend.DTO
{
    public class UserDto
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(30, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

    }
}
