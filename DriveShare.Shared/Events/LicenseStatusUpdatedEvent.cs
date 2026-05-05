namespace DriveShare.Shared.Events
{
    public class LicenseStatusUpdatedEvent
    {
        public int UserId { get; set; }
        public string Status { get; set; } // "Approved" or "Rejected"
        public string Message { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
