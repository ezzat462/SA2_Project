using DriveShare.Shared.Models.Enums;

using System.ComponentModel.DataAnnotations;

namespace DriveShare.UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public required string FullName { get; set; }
        
        [EmailAddress]
        public required string Email { get; set; }
        
        public string? PasswordHash { get; set; }
        
        public UserRole Role { get; set; }
        
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Approved;
        
        public LicenseStatus LicenseStatus { get; set; } = LicenseStatus.Pending;
        
        public string? LicenseImageUrl { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation properties initialized to prevent null
    }
}
