using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.DTOs.Auth;


namespace DriveShare.UserService.Services.Interfaces
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
