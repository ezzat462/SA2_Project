using DriveShare.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveShare.UserService.Models
{
    public class DriverLicense
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public required string LicenseImageUrl { get; set; }
        public LicenseStatus Status { get; set; } = LicenseStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
