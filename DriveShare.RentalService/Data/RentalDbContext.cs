using DriveShare.Shared.Models.Enums;
using DriveShare.RentalService.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.RentalService.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)
        {
        }

        public DbSet<CarPost> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking - CarPost
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Car)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CarPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating - CarPost
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CarPostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Rating - Booking
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal Precision
            modelBuilder.Entity<CarPost>()
                .Property(c => c.PricePerDay)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}
