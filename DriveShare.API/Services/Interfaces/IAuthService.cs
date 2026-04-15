using DriveShare.API.DTOs.Auth;
using DriveShare.API.DTOs.Common;

namespace DriveShare.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<bool>> LogoutAsync(int userId);
        Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(string token, string refreshToken);
        Task<ApiResponse<UserDto>> GetMeAsync(int userId);
    }
}
