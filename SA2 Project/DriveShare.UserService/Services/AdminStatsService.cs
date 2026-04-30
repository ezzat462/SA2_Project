using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.Data;
using DriveShare.UserService.DTOs.Admin;


using DriveShare.UserService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.UserService.Services
{
    public class AdminStatsService : IAdminStatsService
    {
        private readonly ApplicationDbContext _context;

        public AdminStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AdminStatsSummaryDto>> GetSummaryStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalApprovedCars = 0; // TODO: Fetch from RentalService
            var totalActiveRentals = 0; // TODO: Fetch from RentalService
            var totalRevenue = 0m; // TODO: Fetch from RentalService

            var summary = new AdminStatsSummaryDto
            {
                TotalUsers = totalUsers,
                TotalApprovedCars = totalApprovedCars,
                TotalActiveRentals = totalActiveRentals,
                TotalRevenue = totalRevenue
            };

            return ApiResponse<AdminStatsSummaryDto>.SuccessResponse(summary);
        }

        public async Task<ApiResponse<AdminRecentActivityDto>> GetRecentActivityAsync()
        {
            var recentRentals = new List<RentalActivityDto>(); // TODO: Fetch from RentalService

            var newUsers = await _context.Users
                .OrderByDescending(u => u.Id)
                .Take(5)
                .Select(u => new UserRegistrationDto
                {
                    UserId = u.Id,
                    Name = u.FullName,
                    Role = u.Role.ToString()
                })
                .ToListAsync();

            var activity = new AdminRecentActivityDto
            {
                RecentRentals = recentRentals,
                NewUsers = newUsers
            };

            return ApiResponse<AdminRecentActivityDto>.SuccessResponse(activity);
        }
    }
}
