using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.Helpers;
using DriveShare.Shared.Kafka;
using DriveShare.Shared.Events;
using DriveShare.Shared.DTOs.Common;
using DriveShare.UserService.Data;
using DriveShare.UserService.DTOs.Auth;


using DriveShare.UserService.Models;

using DriveShare.UserService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.UserService.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _hasher;
        private readonly IKafkaProducer _kafkaProducer;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper, IConfiguration configuration, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
            _kafkaProducer = kafkaProducer;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<ApiResponse<TokenResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return ApiResponse<TokenResponseDto>.FailureResponse("User with this email already exists");

            if (registerDto.Role == UserRole.Admin)
            {
                var secret = _configuration["AdminSecretCode"] ?? "Admin123!";
                if (string.IsNullOrEmpty(registerDto.AdminSecretCode) || registerDto.AdminSecretCode != secret)
                {
                    return ApiResponse<TokenResponseDto>.FailureResponse("Invalid Admin Secret Code");
                }
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Role = registerDto.Role,
                ApprovalStatus = registerDto.Role == UserRole.CarOwner ? ApprovalStatus.Pending : ApprovalStatus.Approved,
                LicenseStatus = registerDto.Role == UserRole.Renter || registerDto.Role == UserRole.CarOwner ? LicenseStatus.Pending : LicenseStatus.Verified
            };

            user.PasswordHash = _hasher.HashPassword(user, registerDto.Password);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _kafkaProducer.ProduceAsync("user-registered-topic", new UserRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                FullName = user.FullName
            });

            if (user.Role == UserRole.CarOwner)
            {
                // await _notificationService.SendNotificationToAdminsAsync(
                //     $"New owner {user.FullName} has registered and is awaiting approval.",
                //     "AccountPending");
                
                return ApiResponse<TokenResponseDto>.SuccessResponse(null!, "Your account has been created successfully and is pending admin approval.");
            }

            return await GenerateTokenResponse(user);
        }

        public async Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, loginDto.Password) == PasswordVerificationResult.Failed)
                return ApiResponse<TokenResponseDto>.FailureResponse("Invalid credentials");

            if (user.Role == UserRole.CarOwner)
            {
                if (user.ApprovalStatus == ApprovalStatus.Pending)
                    return ApiResponse<TokenResponseDto>.FailureResponse("Your Car Owner account is currently pending admin approval.");
                
                if (user.ApprovalStatus == ApprovalStatus.Rejected)
                    return ApiResponse<TokenResponseDto>.FailureResponse("Your Car Owner account application has been rejected.");
            }

            return await GenerateTokenResponse(user);
        }

        public async Task<ApiResponse<bool>> LogoutAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<bool>.FailureResponse("User not found");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Logged out successfully");
        }

        public async Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(token);
            var userId = int.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return ApiResponse<TokenResponseDto>.FailureResponse("Invalid refresh token");

            return await GenerateTokenResponse(user);
        }

        public async Task<ApiResponse<UserDto>> GetMeAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ApiResponse<UserDto>.FailureResponse("User not found");

            return ApiResponse<UserDto>.SuccessResponse(ToDto(user));
        }

        private async Task<ApiResponse<TokenResponseDto>> GenerateTokenResponse(User user)
        {
            var token = _jwtHelper.CreateToken(user.Id, user.Email, user.FullName, user.Role.ToString(), user.ApprovalStatus.ToString());
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            
            await _context.SaveChangesAsync();

            return ApiResponse<TokenResponseDto>.SuccessResponse(new TokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = ToDto(user)
            });
        }

        private UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ApprovalStatus = user.ApprovalStatus,
            LicenseStatus = user.LicenseStatus,
            LicenseImageUrl = user.LicenseImageUrl
        };
    }
}
