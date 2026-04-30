using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;

using DriveShare.RentalService.Models;


namespace DriveShare.RentalService.Services.Interfaces
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
        
        // Admin Methods
        Task<ApiResponse<bool>> ApproveCarAsync(int carId);
        Task<ApiResponse<bool>> RejectCarAsync(int carId);
        Task<ApiResponse<List<CarPost>>> GetPendingCarsAsync();
    }
}
