using DriveShare.API.Models.Enums;

namespace DriveShare.API.DTOs.Auth
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public UserRole Role { get; set; }
        public string? AdminSecretCode { get; set; }
    }

    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class TokenResponseDto
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public required UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public UserRole Role { get; set; }
        public LicenseStatus LicenseStatus { get; set; }
        public string? LicenseImageUrl { get; set; }
    }
}
