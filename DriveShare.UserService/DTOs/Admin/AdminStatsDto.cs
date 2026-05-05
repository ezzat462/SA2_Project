namespace DriveShare.UserService.DTOs.Admin
{
    public class AdminStatsSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalApprovedCars { get; set; }
        public int TotalActiveRentals { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AdminRecentActivityDto
    {
        public List<RentalActivityDto> RecentRentals { get; set; } = new();
        public List<UserRegistrationDto> NewUsers { get; set; } = new();
    }

    public class RentalActivityDto
    {
        public int RentalId { get; set; }
        public string CarBrand { get; set; } = string.Empty;
        public string RenterName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UserRegistrationDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
