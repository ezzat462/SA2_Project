using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;

namespace DriveShare.API.Services.Interfaces
{
    public interface INotificationService
    {
        // ─── Core: Persist + Real-time to a single user ───
        Task SendNotificationAsync(int userId, string message, string type = "General");

        // ─── Core: Real-time to a SignalR group (also persists for each user in the group) ───
        Task SendNotificationToAdminsAsync(string message, string type = "General");

        // ─── History & Management ───
        Task<ApiResponse<List<Notification>>> GetMyNotificationsAsync(int userId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    }
}
