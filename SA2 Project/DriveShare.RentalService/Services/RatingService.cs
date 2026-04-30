using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;
using DriveShare.RentalService.Data;
using DriveShare.RentalService.DTOs.Rating;
using DriveShare.RentalService.Models;
using DriveShare.RentalService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.RentalService.Services
{
    public class RatingService : IRatingService
    {
        private readonly RentalDbContext _context;

        public RatingService(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<RatingDto>> CreateRatingAsync(RatingRequestDto request, int renterId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId && b.RenterId == renterId);
            if (booking == null) return ApiResponse<RatingDto>.FailureResponse("Booking not found.");

            if (booking.Status != BookingStatus.Completed)
            {
                return ApiResponse<RatingDto>.FailureResponse("You can only rate a completed booking.");
            }

            var existingRating = await _context.Ratings.AnyAsync(r => r.BookingId == request.BookingId);
            if (existingRating)
            {
                return ApiResponse<RatingDto>.FailureResponse("A rating already exists for this booking.");
            }

            var rating = new Rating
            {
                BookingId = booking.Id,
                CarPostId = booking.CarPostId,
                RenterId = renterId,
                Score = request.Score,
                Feedback = request.Feedback,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            var dto = new RatingDto
            {
                Id = rating.Id,
                CarPostId = rating.CarPostId,
                RenterId = rating.RenterId,
                Score = rating.Score,
                Feedback = rating.Feedback,
                CreatedAt = rating.CreatedAt
            };

            return ApiResponse<RatingDto>.SuccessResponse(dto, "Rating submitted successfully.");
        }

        public async Task<ApiResponse<List<RatingDto>>> GetRatingsForCarAsync(int carId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.CarPostId == carId)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    CarPostId = r.CarPostId,
                    RenterId = r.RenterId,
                    RenterName = "User #" + r.RenterId, // TODO: Fetch from UserService via HTTP/gRPC
                    Score = r.Score,
                    Feedback = r.Feedback,
                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<RatingDto>>.SuccessResponse(ratings);
        }
    }
}
