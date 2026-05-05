using DriveShare.NotificationService.Data;
using DriveShare.NotificationService.Models;
using DriveShare.NotificationService.Hubs;
using DriveShare.Shared.DTOs.Common;
using DriveShare.NotificationService.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(NotificationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(int userId, string message, string type = "General")
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var targetGroup = userId == 0 ? "Admins" : $"User_{userId}";
            await _hubContext.Clients.Group(targetGroup).SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                message = notification.Message,
                type = notification.Type,
                createdAt = notification.CreatedAt,
                isRead = notification.IsRead
            });
        }

        public async Task SendNotificationToAdminsAsync(string message, string type = "General")
        {
            var notification = new Notification
            {
                UserId = 0, // 0 represents all Admins
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                message = notification.Message,
                type = notification.Type,
                createdAt = notification.CreatedAt,
                isRead = notification.IsRead
            });
        }

        public async Task<ApiResponse<List<Notification>>> GetMyNotificationsAsync(int userId, bool isAdmin = false)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId || (isAdmin && n.UserId == 0))
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return ApiResponse<List<Notification>>.SuccessResponse(notifications);
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId, bool isAdmin = false)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null || (notification.UserId != userId && !(isAdmin && notification.UserId == 0)))
                return ApiResponse<bool>.FailureResponse("Notification not found.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId, bool isAdmin = false)
        {
            var unread = await _context.Notifications
                .Where(n => (n.UserId == userId || (isAdmin && n.UserId == 0)) && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
