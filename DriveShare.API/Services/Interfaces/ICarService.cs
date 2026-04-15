using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;

namespace DriveShare.API.Services.Interfaces
{
    public interface ICarService
    {
        Task<ApiResponse<PaginatedResult<CarPost>>> GetAllApprovedCarsAsync(string? brand = null, string? location = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, int pageSize = 10);
        Task<ApiResponse<CarPost>> GetCarByIdAsync(int id);
        Task<ApiResponse<CarPost>> CreateCarAsync(CarPost car);
        Task<ApiResponse<List<CarPost>>> GetCarsByOwnerAsync(int ownerId);
    }
}
