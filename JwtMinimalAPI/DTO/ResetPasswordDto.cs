namespace JwtMinimalAPI.DTO
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public Guid ForgotPasswordToken { get; set; }
        public DateTime? ExpireTime { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
