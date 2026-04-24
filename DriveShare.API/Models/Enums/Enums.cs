namespace DriveShare.API.Models.Enums
{
    public enum UserRole
    {
        Admin,
        CarOwner,
        Renter
    }

    public enum LicenseStatus
    {
        Pending,
        Verified,
        Rejected
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum CarStatus
    {
        Available,
        Rented,
        Maintenance
    }

    public enum BookingStatus
    {
        Pending,
        Accepted,
        Rejected,
        Completed,
        Cancelled
    }

    public enum TransmissionType
    {
        Auto,
        Manual
    }

    public enum CarType
    {
        Sedan,
        SUV,
        Truck,
        Coupe,
        Convertible,
        Van,
        Other
    }
}
