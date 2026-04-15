using DriveShare.API.DTOs.Admin;
using DriveShare.API.DTOs.Common;

namespace DriveShare.API.Services.Interfaces
{
    public interface IAdminStatsService
    {
        Task<ApiResponse<AdminStatsSummaryDto>> GetSummaryStatsAsync();
        Task<ApiResponse<AdminRecentActivityDto>> GetRecentActivityAsync();
    }
}
