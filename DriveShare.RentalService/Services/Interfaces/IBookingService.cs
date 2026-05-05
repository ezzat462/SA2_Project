using DriveShare.Shared.DTOs.Common;
using DriveShare.RentalService.DTOs.Booking;


namespace DriveShare.RentalService.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingDto>> CreateBookingAsync(BookingRequestDto request, int renterId);
        Task<ApiResponse<bool>> AcceptBookingAsync(int bookingId, int ownerId);
        Task<ApiResponse<bool>> RejectBookingAsync(int bookingId, int ownerId);
        Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync(int renterId);
        Task<ApiResponse<List<BookingDto>>> GetIncomingBookingsAsync(int ownerId);
    }
}
