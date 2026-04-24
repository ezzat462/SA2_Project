namespace DriveShare.API.DTOs.Rating
{
    public class RatingRequestDto
    {
        public int BookingId { get; set; }
        public int Score { get; set; }
        public string? Feedback { get; set; }
    }

    public class RatingDto
    {
        public int Id { get; set; }
        public int CarPostId { get; set; }
        public int RenterId { get; set; }
        public string RenterName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
