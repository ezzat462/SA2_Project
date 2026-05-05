using DriveShare.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveShare.RentalService.Models
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
        // No User navigation — User lives in UserService

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
        // No User navigation — User lives in UserService

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
        // No User navigation — User lives in UserService

        public int Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
