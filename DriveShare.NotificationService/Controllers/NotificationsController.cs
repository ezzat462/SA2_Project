using DriveShare.NotificationService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.NotificationService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || 
                          User.HasClaim(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            var response = await _notificationService.GetMyNotificationsAsync(userId, isAdmin);
            return Ok(response);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || 
                          User.HasClaim(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            var response = await _notificationService.MarkAsReadAsync(id, userId, isAdmin);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || 
                          User.HasClaim(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            var response = await _notificationService.MarkAllAsReadAsync(userId, isAdmin);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
