using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.DTOs.Admin;


namespace DriveShare.UserService.Services.Interfaces
{
    public interface IAdminStatsService
    {
        Task<ApiResponse<AdminStatsSummaryDto>> GetSummaryStatsAsync();
        Task<ApiResponse<AdminRecentActivityDto>> GetRecentActivityAsync();
    }
}
