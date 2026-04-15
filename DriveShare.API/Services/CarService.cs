using DriveShare.API.Data;
using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;
using DriveShare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.API.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PaginatedResult<CarPost>>> GetAllApprovedCarsAsync(string? brand = null, string? location = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Cars
                .Where(c => c.IsApproved)
                .Include(c => c.Owner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(c => c.Brand.Contains(brand));

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(c => c.Location.Contains(location));

            if (minPrice.HasValue)
                query = query.Where(c => c.PricePerDay >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<CarPost>
            {
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                Items = items
            };

            return ApiResponse<PaginatedResult<CarPost>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<CarPost>> GetCarByIdAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (car == null) return ApiResponse<CarPost>.FailureResponse("Car not found");
            return ApiResponse<CarPost>.SuccessResponse(car);
        }

        public async Task<ApiResponse<CarPost>> CreateCarAsync(CarPost car)
        {
            // Validate availability window
            var today = DateTime.UtcNow.Date;
            if (car.AvailableFrom.Date < today)
                return ApiResponse<CarPost>.FailureResponse("Available From date cannot be in the past.");

            if (car.AvailableTo.Date <= car.AvailableFrom.Date)
                return ApiResponse<CarPost>.FailureResponse("Available To date must be after Available From date.");

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return ApiResponse<CarPost>.SuccessResponse(car, "Car posted successfully. Waiting for admin approval.");
        }

        public async Task<ApiResponse<List<CarPost>>> GetCarsByOwnerAsync(int ownerId)
        {
            var cars = await _context.Cars.Where(c => c.OwnerId == ownerId).ToListAsync();
            return ApiResponse<List<CarPost>>.SuccessResponse(cars);
        }
    }
}
