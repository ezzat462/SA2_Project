using DriveShare.API.Models;
using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.API.Controllers
{
    [ApiController]
    [Route("api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? brand, 
            [FromQuery] string? location, 
            [FromQuery] decimal? minPrice, 
            [FromQuery] decimal? maxPrice, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var response = await _carService.GetAllApprovedCarsAsync(brand, location, minPrice, maxPrice, page, pageSize);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _carService.GetCarByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [Authorize(Roles = "CarOwner,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CarPost car)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            car.OwnerId = int.Parse(userIdClaim);
            car.IsApproved = false; // Always false for new posts
            
            var response = await _carService.CreateCarAsync(car);
            return Ok(response);
        }

        [Authorize(Roles = "CarOwner,Admin")]
        [HttpGet("my-cars")]
        public async Task<IActionResult> GetMyCars()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var response = await _carService.GetCarsByOwnerAsync(int.Parse(userIdClaim));
            return Ok(response);
        }
    }
}
