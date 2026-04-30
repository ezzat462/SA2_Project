using DriveShare.Shared.DTOs.Common;

using DriveShare.RentalService.DTOs.Rating;

namespace DriveShare.RentalService.Services.Interfaces
{
    public interface IRatingService
    {
        Task<ApiResponse<RatingDto>> CreateRatingAsync(RatingRequestDto request, int renterId);
        Task<ApiResponse<List<RatingDto>>> GetRatingsForCarAsync(int carId);
    }
}
