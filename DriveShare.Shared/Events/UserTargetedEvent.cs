namespace DriveShare.Shared.Events
{
    public class UserTargetedEvent
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
