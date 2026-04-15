using DriveShare.API.DTOs.Admin;
using DriveShare.API.DTOs.Common;

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
    }

    public interface ILicenseService
    {
        Task<ApiResponse<string>> UploadLicenseAsync(IFormFile file, int userId);
    }
}
