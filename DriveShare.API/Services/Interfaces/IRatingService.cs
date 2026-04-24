using DriveShare.API.DTOs.Common;
using DriveShare.API.DTOs.Rating;

namespace DriveShare.API.Services.Interfaces
{
    public interface IRatingService
    {
        Task<ApiResponse<RatingDto>> CreateRatingAsync(RatingRequestDto request, int renterId);
        Task<ApiResponse<List<RatingDto>>> GetRatingsForCarAsync(int carId);
    }
}
