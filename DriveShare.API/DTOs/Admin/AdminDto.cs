using DriveShare.API.Models.Enums;

namespace DriveShare.API.DTOs.Admin
{
    public class PendingUserDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public LicenseStatus LicenseStatus { get; set; }
        public required string LicenseImageUrl { get; set; }
    }

    public class PendingCarDto
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public required string Location { get; set; }
        public required string OwnerName { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class PendingOwnerDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}
