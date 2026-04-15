using DriveShare.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DriveShare.API.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public required string FullName { get; set; }
        
        [EmailAddress]
        public required string Email { get; set; }
        
        public string? PasswordHash { get; set; }
        
        public UserRole Role { get; set; }
        
        public LicenseStatus LicenseStatus { get; set; } = LicenseStatus.Pending;
        
        public string? LicenseImageUrl { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation properties initialized to prevent null reference issues
        public ICollection<CarPost> CarsOwned { get; set; } = new List<CarPost>();
        public ICollection<RentalRequest> RentalsRequested { get; set; } = new List<RentalRequest>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
