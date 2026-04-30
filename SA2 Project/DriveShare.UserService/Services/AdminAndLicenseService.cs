using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.Data;
using DriveShare.UserService.DTOs.Admin;

using DriveShare.UserService.Models;

using DriveShare.UserService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.UserService.Services
{
    // ═══════════════════════════════════════════════════════════════
    //  AdminService — Car & Owner approval workflows
    // ═══════════════════════════════════════════════════════════════
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── License (User) Approval ──────────────────────────────
        public async Task<ApiResponse<bool>> ApproveUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Verified;
            await _context.SaveChangesAsync();

            // await _notificationService.SendNotificationAsync(userId,
            //     "Your license has been verified successfully! You can now proceed to book cars.",
            //     "LicenseApproved");

            return ApiResponse<bool>.SuccessResponse(true, "User approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Rejected;
            await _context.SaveChangesAsync();

            // await _notificationService.SendNotificationAsync(userId,
            //     "Your driver's license has been rejected. Please re-upload a valid document.",
            //     "LicenseRejected");

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


        // ── Owner Account Approval ───────────────────────────────
        public async Task<ApiResponse<List<PendingOwnerDto>>> GetPendingOwnersAsync()
        {
            var owners = await _context.Users
                .Where(u => u.Role == UserRole.CarOwner && u.ApprovalStatus == ApprovalStatus.Pending)
                .Select(u => new PendingOwnerDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    ApprovalStatus = u.ApprovalStatus
                })
                .ToListAsync();

            return ApiResponse<List<PendingOwnerDto>>.SuccessResponse(owners);
        }

        public async Task<ApiResponse<bool>> UpdateOwnerStatusAsync(int userId, ApprovalStatus status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Role != UserRole.CarOwner)
                return ApiResponse<bool>.FailureResponse("Owner not found");

            user.ApprovalStatus = status;
            await _context.SaveChangesAsync();

            if (status == ApprovalStatus.Approved)
            {
                // await _notificationService.SendNotificationAsync(userId,
                //     "Your owner account has been approved! You can now list vehicles on DriveShare.",
                //     "AccountApproved");
            }
            else if (status == ApprovalStatus.Rejected)
            {
                // await _notificationService.SendNotificationAsync(userId,
                //     "Your owner account application has been rejected.",
                //     "AccountRejected");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Owner status updated successfully");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  LicenseService — Upload & Verification workflow
    // ═══════════════════════════════════════════════════════════════
    public class LicenseService : ILicenseService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        public LicenseService(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        // ───────────────────────────────────────────────────────────
        //  WORKFLOW 2A: License Upload
        //  ► Saves the file, creates a DriverLicense record
        //  ► Notifies ALL admins: "User [Name] has uploaded a license for verification."
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<string>> UploadLicenseAsync(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<string>.FailureResponse("No file uploaded.");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/{fileName}";

            // Create the DriverLicense record
            var license = new DriverLicense
            {
                UserId = userId,
                LicenseImageUrl = fileUrl,
                Status = LicenseStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };
            
            _context.DriverLicenses.Add(license);

            // Sync license info to User model
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LicenseImageUrl = fileUrl;
                user.LicenseStatus = LicenseStatus.Pending;
            }
            await _context.SaveChangesAsync();

            // ► NOTIFICATION: Admins — "User [Name] has uploaded a license for verification."
            // await _notificationService.SendNotificationToAdminsAsync(
            //     $"User {user?.FullName ?? "Unknown"} has uploaded a license for verification.",
            //     "LicenseUploaded");

            return ApiResponse<string>.SuccessResponse(fileUrl, "License uploaded successfully! It is now under review.");
        }

        public async Task<ApiResponse<DriverLicense>> GetMyLicenseAsync(int userId)
        {
            var license = await _context.DriverLicenses
                .OrderByDescending(l => l.SubmittedAt)
                .FirstOrDefaultAsync(l => l.UserId == userId);
            
            return ApiResponse<DriverLicense>.SuccessResponse(license!);
        }

        // ───────────────────────────────────────────────────────────
        //  WORKFLOW 2B: License Verification (Admin Approves)
        //  ► Updates the license + user status
        //  ► Notifies user: "Your license has been verified successfully! You can now proceed to book cars."
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<bool>> VerifyLicenseAsync(int licenseId)
        {
            var license = await _context.DriverLicenses
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return ApiResponse<bool>.FailureResponse("License not found.");

            license.Status = LicenseStatus.Verified;
            if (license.User != null)
            {
                license.User.LicenseStatus = LicenseStatus.Verified;
            }
            
            await _context.SaveChangesAsync();

            // ► NOTIFICATION: User — Approval message
            // await _notificationService.SendNotificationAsync(license.UserId, 
            //     "Your license has been verified successfully! You can now proceed to book cars.", 
            //     "LicenseApproved");

            return ApiResponse<bool>.SuccessResponse(true, "License verified successfully.");
        }

        public async Task<ApiResponse<bool>> RejectLicenseAsync(int licenseId)
        {
            var license = await _context.DriverLicenses
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return ApiResponse<bool>.FailureResponse("License not found.");

            license.Status = LicenseStatus.Rejected;
            if (license.User != null)
            {
                license.User.LicenseStatus = LicenseStatus.Rejected;
            }
            
            await _context.SaveChangesAsync();

            // ► NOTIFICATION: User — Rejection message
            // await _notificationService.SendNotificationAsync(license.UserId, 
            //     "Your driver's license has been rejected. Please upload a valid document.", 
            //     "LicenseRejected");

            return ApiResponse<bool>.SuccessResponse(true, "License rejected.");
        }

        public async Task<ApiResponse<List<DriverLicense>>> GetPendingLicensesAsync()
        {
            var licenses = await _context.DriverLicenses
                .Include(l => l.User)
                .Where(l => l.Status == LicenseStatus.Pending)
                .ToListAsync();
                
            return ApiResponse<List<DriverLicense>>.SuccessResponse(licenses);
        }
    }
}
