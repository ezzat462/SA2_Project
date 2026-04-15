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
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public required string Location { get; set; }
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

        public ICollection<RentalRequest> RentalRequests { get; set; } = new List<RentalRequest>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }

    public class RentalRequest
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        [ForeignKey("CarId")]
        public CarPost? Car { get; set; }

        public int RenterId { get; set; }
        [ForeignKey("RenterId")]
        public User? Renter { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public RentalStatus Status { get; set; } = RentalStatus.Pending;
    }

    public class Rating
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        [ForeignKey("CarId")]
        public CarPost? Car { get; set; }

        public int RenterId { get; set; }
        [ForeignKey("RenterId")]
        public User? Renter { get; set; }

        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
