using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveShare.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminStatsService _adminStatsService;
        private readonly ILicenseService _licenseService;

        public AdminController(IAdminService adminService, IAdminStatsService adminStatsService, ILicenseService licenseService)
        {
            _adminService = adminService;
            _adminStatsService = adminStatsService;
            _licenseService = licenseService;
        }

        [HttpGet("stats/summary")]
        public async Task<IActionResult> GetSummaryStats()
        {
            var response = await _adminStatsService.GetSummaryStatsAsync();
            return Ok(response);
        }

        [HttpGet("stats/recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var response = await _adminStatsService.GetRecentActivityAsync();
            return Ok(response);
        }

        [HttpPut("users/{id}/approve")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var response = await _adminService.ApproveUserAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("users/{id}/reject")]
        public async Task<IActionResult> RejectUser(int id)
        {
            var response = await _adminService.RejectUserAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("users/pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var response = await _adminService.GetPendingUsersAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("owners/pending")]
        public async Task<IActionResult> GetPendingOwners()
        {
            var response = await _adminService.GetPendingOwnersAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("owners/{id}/status")]
        public async Task<IActionResult> UpdateOwnerStatus(int id, [FromBody] DriveShare.API.Models.Enums.ApprovalStatus status)
        {
            var response = await _adminService.UpdateOwnerStatusAsync(id, status);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("cars/{id}/approve")]
        public async Task<IActionResult> ApproveCar(int id)
        {
            var response = await _adminService.ApproveCarAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("cars/{id}/reject")]
        public async Task<IActionResult> RejectCar(int id)
        {
            var response = await _adminService.RejectCarAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("cars/pending")]
        public async Task<IActionResult> GetPendingCars()
        {
            var response = await _adminService.GetPendingCarsAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("license/{id}/verify")]
        public async Task<IActionResult> VerifyLicense(int id)
        {
            var response = await _licenseService.VerifyLicenseAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("license/{id}/reject")]
        public async Task<IActionResult> RejectLicense(int id)
        {
            var response = await _licenseService.RejectLicenseAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("license/pending")]
        public async Task<IActionResult> GetPendingLicenses()
        {
            var response = await _licenseService.GetPendingLicensesAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
