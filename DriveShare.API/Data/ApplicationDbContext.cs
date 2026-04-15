using DriveShare.API.Models;
using DriveShare.API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CarPost> Cars { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CarPost - User (Owner)
            modelBuilder.Entity<CarPost>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.CarsOwned)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.NoAction); // Avoid cascade path

            // RentalRequest - User (Renter)
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.Renter)
                .WithMany(u => u.RentalsRequested)
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // RentalRequest - CarPost
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.Car)
                .WithMany(c => c.RentalRequests)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating - User (Renter)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Renter)
                .WithMany()
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal Precision
            modelBuilder.Entity<CarPost>()
                .Property(c => c.PricePerDay)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RentalRequest>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

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
