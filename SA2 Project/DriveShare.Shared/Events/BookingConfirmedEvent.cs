namespace DriveShare.Shared.Events
{
    public class BookingConfirmedEvent
    {
        public int BookingId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CarId { get; set; }
    }
}
