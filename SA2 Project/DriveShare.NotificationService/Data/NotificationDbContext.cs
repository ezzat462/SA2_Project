using DriveShare.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
    }
}
