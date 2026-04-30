using DriveShare.Shared.Models.Enums;
using DriveShare.UserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.UserService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Admin User
            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = 1,
                FullName = "Admin User",
                Email = "admin@driveshare.com",
                Role = UserRole.Admin,
                LicenseStatus = LicenseStatus.Verified,
                PasswordHash = hasher.HashPassword(null!, "Admin123!") // Explicit null suppress for the seed
            };

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
