using DriveShare.API.Data;
using DriveShare.API.DTOs.Admin;
using DriveShare.API.DTOs.Common;
using DriveShare.API.Models.Enums;
using DriveShare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.API.Services
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
            var totalApprovedCars = await _context.Cars.CountAsync(c => c.IsApproved);
            
            var totalActiveRentals = await _context.RentalRequests
                .CountAsync(r => r.Status == RentalStatus.Approved || r.Status == RentalStatus.Pending);
            
            // Simple revenue calculation (sum of all rentals)
            var totalRevenue = await _context.RentalRequests
                .SumAsync(r => (decimal?)r.TotalPrice) ?? 0m;

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
            var recentRentals = await _context.RentalRequests
                .Include(r => r.Car)
                .Include(r => r.Renter)
                .OrderByDescending(r => r.Id)
                .Take(5)
                .Select(r => new RentalActivityDto
                {
                    RentalId = r.Id,
                    CarBrand = r.Car!.Brand + " " + r.Car.Model,
                    RenterName = r.Renter!.FullName,
                    Status = r.Status.ToString()
                })
                .ToListAsync();

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
