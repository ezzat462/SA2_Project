using DriveShare.Shared.DTOs.Common;
using DriveShare.NotificationService.Models;

namespace DriveShare.NotificationService.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string message, string type = "General");
        Task SendNotificationToAdminsAsync(string message, string type = "General");
        Task<ApiResponse<List<Notification>>> GetMyNotificationsAsync(int userId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    }
}
