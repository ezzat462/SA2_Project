using DriveShare.API.Data;
using DriveShare.API.DTOs.Common;
using DriveShare.API.Hubs;
using DriveShare.API.Models;
using DriveShare.API.Models.Enums;
using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // ───────────────────────────────────────────────────────────
        //  Persist to DB + Emit SignalR to a single user
        // ───────────────────────────────────────────────────────────
        public async Task SendNotificationAsync(int userId, string message, string type = "General")
        {
            // 1. Persist
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

            // 2. Emit via SignalR (target the user's private group)
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                message = notification.Message,
                type = notification.Type,
                createdAt = notification.CreatedAt,
                isRead = notification.IsRead
            });
        }

        // ───────────────────────────────────────────────────────────
        //  Persist for every Admin user + Emit to "Admins" group
        // ───────────────────────────────────────────────────────────
        public async Task SendNotificationToAdminsAsync(string message, string type = "General")
        {
            var adminIds = await _context.Users
                .Where(u => u.Role == UserRole.Admin)
                .Select(u => u.Id)
                .ToListAsync();

            // 1. Persist one notification row per admin
            foreach (var adminId in adminIds)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = adminId,
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }
            await _context.SaveChangesAsync();

            // 2. Single SignalR emit to the Admins group (every connected admin receives it)
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", new
            {
                id = 0,          // Group emission — individual IDs are in the DB rows above
                message,
                type,
                createdAt = DateTime.UtcNow,
                isRead = false
            });
        }

        // ───────────────────────────────────────────────────────────
        //  History: Fetch the latest 50 notifications for a user
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<List<Notification>>> GetMyNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return ApiResponse<List<Notification>>.SuccessResponse(notifications);
        }

        // ───────────────────────────────────────────────────────────
        //  Mark a single notification as read
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null || notification.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Notification not found.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true);
        }

        // ───────────────────────────────────────────────────────────
        //  Mark all unread notifications as read for a user
        // ───────────────────────────────────────────────────────────
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
