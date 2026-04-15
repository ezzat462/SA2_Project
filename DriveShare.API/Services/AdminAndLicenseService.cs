using DriveShare.API.Data;
using DriveShare.API.DTOs.Admin;
using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;
using DriveShare.API.Models.Enums;
using DriveShare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<bool>> ApproveUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Verified;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "User approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Rejected;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "User rejected successfully");
        }

        public async Task<ApiResponse<List<PendingUserDto>>> GetPendingUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.LicenseStatus == LicenseStatus.Pending && !string.IsNullOrEmpty(u.LicenseImageUrl))
                .Select(u => new PendingUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    LicenseStatus = u.LicenseStatus,
                    LicenseImageUrl = u.LicenseImageUrl ?? string.Empty
                }).ToListAsync();

            return ApiResponse<List<PendingUserDto>>.SuccessResponse(users);
        }

        public async Task<ApiResponse<bool>> ApproveCarAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return ApiResponse<bool>.FailureResponse("Car not found");

            car.IsApproved = true;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Car approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectCarAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return ApiResponse<bool>.FailureResponse("Car not found");

            _context.Cars.Remove(car); // Or set status to rejected
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Car rejected successfully");
        }

        public async Task<ApiResponse<List<PendingCarDto>>> GetPendingCarsAsync()
        {
            var cars = await _context.Cars
                .Include(c => c.Owner)
                .Where(c => !c.IsApproved)
                .Select(c => new PendingCarDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    PricePerDay = c.PricePerDay,
                    Location = c.Location,
                    OwnerName = c.Owner!.FullName,
                    ImageUrl = c.ImageUrl
                }).ToListAsync();

            return ApiResponse<List<PendingCarDto>>.SuccessResponse(cars);
        }
    }

    public class LicenseService : ILicenseService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public LicenseService(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        public async Task<ApiResponse<string>> UploadLicenseAsync(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<string>.FailureResponse("No file uploaded");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/{fileName}";

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LicenseImageUrl = fileUrl;
                user.LicenseStatus = LicenseStatus.Pending;
                await _context.SaveChangesAsync();
            }

            return ApiResponse<string>.SuccessResponse(fileUrl, "License uploaded successfully");
        }
    }
}
