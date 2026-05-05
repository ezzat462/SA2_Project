using System.ComponentModel.DataAnnotations;

namespace DriveShare.NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Message { get; set; }
        public string Type { get; set; } = "General";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
