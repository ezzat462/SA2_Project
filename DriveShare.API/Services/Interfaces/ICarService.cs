using DriveShare.API.DTOs.Common;
using DriveShare.API.Models;
using DriveShare.API.Models.Enums;

namespace DriveShare.API.Services.Interfaces
{
    public interface ICarService
    {
        Task<ApiResponse<PaginatedResult<CarPost>>> GetAllApprovedCarsAsync(
            string? brand = null, 
            string? location = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            string? sortOrder = null, 
            CarType? carType = null,
            TransmissionType? transmission = null,
            int page = 1, 
            int pageSize = 10);
        Task<ApiResponse<CarPost>> GetCarByIdAsync(int id);
        Task<ApiResponse<CarPost>> CreateCarAsync(CarPost car);
        Task<ApiResponse<CarPost>> UpdateCarAsync(int id, CarPost car);
        Task<ApiResponse<bool>> DeleteCarAsync(int carId, int ownerId);
        Task<ApiResponse<List<CarPost>>> GetCarsByOwnerAsync(int ownerId);
    }
}
