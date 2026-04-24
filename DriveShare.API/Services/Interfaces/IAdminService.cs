using DriveShare.API.DTOs.Admin;
using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;
using DriveShare.API.Models.Enums;

namespace DriveShare.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<bool>> ApproveUserAsync(int userId);
        Task<ApiResponse<bool>> RejectUserAsync(int userId);
        Task<ApiResponse<List<PendingUserDto>>> GetPendingUsersAsync();

        Task<ApiResponse<bool>> ApproveCarAsync(int carId);
        Task<ApiResponse<bool>> RejectCarAsync(int carId);
        Task<ApiResponse<List<PendingCarDto>>> GetPendingCarsAsync();

        Task<ApiResponse<List<PendingOwnerDto>>> GetPendingOwnersAsync();
        Task<ApiResponse<bool>> UpdateOwnerStatusAsync(int userId, ApprovalStatus status);
    }

    public interface ILicenseService
    {
        Task<ApiResponse<string>> UploadLicenseAsync(IFormFile file, int userId);
        Task<ApiResponse<DriverLicense>> GetMyLicenseAsync(int userId);
        Task<ApiResponse<bool>> VerifyLicenseAsync(int licenseId);
        Task<ApiResponse<bool>> RejectLicenseAsync(int licenseId);
        Task<ApiResponse<List<DriverLicense>>> GetPendingLicensesAsync();
    }
}
