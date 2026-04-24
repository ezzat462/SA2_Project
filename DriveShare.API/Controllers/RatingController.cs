using DriveShare.API.DTOs.Rating;
using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost("ratings")]
        [Authorize(Roles = "Renter")]
        public async Task<IActionResult> CreateRating([FromBody] RatingRequestDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _ratingService.CreateRatingAsync(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("cars/{id}/ratings")]
        public async Task<IActionResult> GetRatingsForCar(int id)
        {
            var response = await _ratingService.GetRatingsForCarAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
