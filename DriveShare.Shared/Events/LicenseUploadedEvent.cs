namespace DriveShare.Shared.Events
{
    public class LicenseUploadedEvent
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LicenseImageUrl { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
