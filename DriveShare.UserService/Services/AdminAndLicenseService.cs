using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;
using DriveShare.Shared.Kafka;
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
        private readonly IKafkaProducer _kafkaProducer;

        public AdminService(ApplicationDbContext context, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        // ── License (User) Approval ──────────────────────────────
        public async Task<ApiResponse<bool>> ApproveUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Verified;
            await _context.SaveChangesAsync();

            await _kafkaProducer.ProduceAsync("user-targeted-topic", new DriveShare.Shared.Events.UserTargetedEvent
            {
                UserId = user.Id,
                Type = "AccountApproved",
                Message = "Your license has been verified. You can now book cars!"
            });

            return ApiResponse<bool>.SuccessResponse(true, "User approved successfully");
        }

        public async Task<ApiResponse<bool>> RejectUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.LicenseStatus = LicenseStatus.Rejected;
            await _context.SaveChangesAsync();

            await _kafkaProducer.ProduceAsync("user-targeted-topic", new DriveShare.Shared.Events.UserTargetedEvent
            {
                UserId = user.Id,
                Type = "AccountRejected",
                Message = "Your license verification was rejected. Please contact support."
            });

            return ApiResponse<bool>.SuccessResponse(true, "User rejected successfully");
        }

        public async Task<ApiResponse<List<PendingUserDto>>> GetPendingUsersAsync()
        {
            // Refactored to fetch from DriverLicenses table as Source of Truth
            var pendingUsers = await _context.DriverLicenses
                .Include(l => l.User)
                .Where(l => l.Status == LicenseStatus.Pending)
                .Select(l => new PendingUserDto
                {
                    Id = l.UserId,
                    FullName = l.User != null ? l.User.FullName : "Unknown",
                    Email = l.User != null ? l.User.Email : "Unknown",
                    LicenseStatus = l.Status,
                    LicenseImageUrl = string.IsNullOrEmpty(l.LicenseImageUrl) 
                        ? "" 
                        : (l.LicenseImageUrl.StartsWith("http") 
                            ? l.LicenseImageUrl 
                            : $"http://localhost:5001/uploads/licenses/{Path.GetFileName(l.LicenseImageUrl)}")
                }).ToListAsync();

            return ApiResponse<List<PendingUserDto>>.SuccessResponse(pendingUsers);
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

            var eventType = status == ApprovalStatus.Approved ? "AccountApproved" : "AccountRejected";
            var eventMsg = status == ApprovalStatus.Approved 
                ? "Your owner account has been approved! You can now post car listings." 
                : "Your owner account request was rejected. Please contact support.";
                
            await _kafkaProducer.ProduceAsync("user-targeted-topic", new DriveShare.Shared.Events.UserTargetedEvent
            {
                UserId = user.Id,
                Type = eventType,
                Message = eventMsg
            });

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
        private readonly IKafkaProducer _kafkaProducer;

        public LicenseService(IWebHostEnvironment environment, ApplicationDbContext context, IKafkaProducer kafkaProducer)
        {
            _environment = environment;
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<ApiResponse<string>> UploadLicenseAsync(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<string>.FailureResponse("No file uploaded.");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "licenses");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/licenses/{fileName}";
            var fullUrl = $"http://localhost:5001{fileUrl}";

            var license = new DriverLicense
            {
                UserId = userId,
                LicenseImageUrl = fullUrl,
                Status = LicenseStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };
            
            _context.DriverLicenses.Add(license);

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LicenseImageUrl = fullUrl;
                user.LicenseStatus = LicenseStatus.Pending;
            }
            await _context.SaveChangesAsync();


            Console.WriteLine($"[KAFKA] Publishing fallback event for User: {userId} to topic: user-targeted-topic");
            await _kafkaProducer.ProduceAsync("user-targeted-topic", new DriveShare.Shared.Events.UserTargetedEvent
            {
                UserId = 0,
                Type = "LicenseUploaded",
                Message = $"New license verification request: {user?.FullName ?? "Unknown"} has uploaded a document."
            });
            
            Console.WriteLine($"[KAFKA] Events published successfully for User: {userId}");

            return ApiResponse<string>.SuccessResponse(fullUrl, "License uploaded successfully! It is now under review.");
        }

        public async Task<ApiResponse<DriverLicense>> GetMyLicenseAsync(int userId)
        {
            var license = await _context.DriverLicenses
                .OrderByDescending(l => l.SubmittedAt)
                .FirstOrDefaultAsync(l => l.UserId == userId);
            
            if (license != null && !string.IsNullOrEmpty(license.LicenseImageUrl) && !license.LicenseImageUrl.StartsWith("http"))
            {
                license.LicenseImageUrl = $"http://localhost:5001/uploads/licenses/{Path.GetFileName(license.LicenseImageUrl)}";
            }

            return ApiResponse<DriverLicense>.SuccessResponse(license!);
        }

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
            
            Console.WriteLine($"[KAFKA] Publishing LicenseStatusUpdatedEvent (Approved) for User: {license.UserId} to topic: license-status-topic");
            await _kafkaProducer.ProduceAsync("license-status-topic", new DriveShare.Shared.Events.LicenseStatusUpdatedEvent
            {
                UserId = license.UserId,
                Status = "Approved",
                Message = "Your driver's license has been verified successfully."
            });

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
            
            Console.WriteLine($"[KAFKA] Publishing LicenseStatusUpdatedEvent (Rejected) for User: {license.UserId} to topic: license-status-topic");
            await _kafkaProducer.ProduceAsync("license-status-topic", new DriveShare.Shared.Events.LicenseStatusUpdatedEvent
            {
                UserId = license.UserId,
                Status = "Rejected",
                Message = "Your driver's license has been rejected. Please re-upload a clear copy."
            });

            return ApiResponse<bool>.SuccessResponse(true, "License rejected.");
        }

        public async Task<ApiResponse<List<DriverLicense>>> GetPendingLicensesAsync()
        {
            var licenses = await _context.DriverLicenses
                .Include(l => l.User)
                .Where(l => l.Status == LicenseStatus.Pending)
                .ToListAsync();

            foreach (var license in licenses)
            {
                if (!string.IsNullOrEmpty(license.LicenseImageUrl) && !license.LicenseImageUrl.StartsWith("http"))
                {
                    license.LicenseImageUrl = $"http://localhost:5001/uploads/licenses/{Path.GetFileName(license.LicenseImageUrl)}";
                }
            }
                
            return ApiResponse<List<DriverLicense>>.SuccessResponse(licenses);
        }
    }
}
