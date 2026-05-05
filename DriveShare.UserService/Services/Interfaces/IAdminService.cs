using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.DTOs.Admin;

using DriveShare.UserService.Models;


namespace DriveShare.UserService.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<bool>> ApproveUserAsync(int userId);
        Task<ApiResponse<bool>> RejectUserAsync(int userId);
        Task<ApiResponse<List<PendingUserDto>>> GetPendingUsersAsync();


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
