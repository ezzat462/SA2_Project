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

            await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
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
            // In a real microservice, we might not have all user IDs here.
            // But for now, we'll keep the broadcast logic.
            // In a true decoupled system, we might just emit to the "Admins" group
            // and let the client handle it, or have the UserService publish an event.
            
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", new
            {
                id = 0,
                message,
                type,
                createdAt = DateTime.UtcNow,
                isRead = false
            });
        }

        public async Task<ApiResponse<List<Notification>>> GetMyNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return ApiResponse<List<Notification>>.SuccessResponse(notifications);
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null || notification.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Notification not found.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
