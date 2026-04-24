using DriveShare.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveShare.API.Models
{
    public class CarPost
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string Title { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public required string Location { get; set; }
        public CarType Type { get; set; } = CarType.Other;
        public TransmissionType Transmission { get; set; } = TransmissionType.Auto;
        public bool IsApproved { get; set; } = false;
        public CarStatus Status { get; set; } = CarStatus.Available;

        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        /// <summary>Start of the window when this car is available for rent.</summary>
        public DateTime AvailableFrom { get; set; }

        /// <summary>End of the window when this car is available for rent.</summary>
        public DateTime AvailableTo { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }

    public class Booking
    {
        public int Id { get; set; }
        public int CarPostId { get; set; }
        [ForeignKey("CarPostId")]
        public CarPost? Car { get; set; }

        public int RenterId { get; set; }
        [ForeignKey("RenterId")]
        public User? Renter { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }

    public class Rating
    {
        public int Id { get; set; }
        
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        public int CarPostId { get; set; }
        [ForeignKey("CarPostId")]
        public CarPost? Car { get; set; }

        public int RenterId { get; set; }
        [ForeignKey("RenterId")]
        public User? Renter { get; set; }

        public int Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public required string Message { get; set; }

        /// <summary>Categorizes the notification for frontend routing (e.g., NewCarPost, LicenseApproved, BookingResolution).</summary>
        public string Type { get; set; } = "General";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }

    public class DriverLicense
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public required string LicenseImageUrl { get; set; }
        public LicenseStatus Status { get; set; } = LicenseStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
