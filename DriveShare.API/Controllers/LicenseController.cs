using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.API.Controllers
{
    [ApiController]
    [Route("api/license")]
    [Authorize]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadLicense(IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var response = await _licenseService.UploadLicenseAsync(file, int.Parse(userIdClaim));
            
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
