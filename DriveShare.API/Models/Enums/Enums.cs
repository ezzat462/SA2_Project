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

    public enum CarStatus
    {
        Available,
        Rented,
        Maintenance
    }

    public enum RentalStatus
    {
        Pending,
        Approved,
        Rejected,
        Completed,
        Cancelled
    }
}
