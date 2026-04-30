using DriveShare.RentalService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveShare.RentalService.Controllers
{
    [ApiController]
    [Route("api/admin/cars")]
    [Authorize(Roles = "Admin")]
    public class AdminCarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public AdminCarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCars()
        {
            var response = await _carService.GetPendingCarsAsync();
            return Ok(response);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveCar(int id)
        {
            var response = await _carService.ApproveCarAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectCar(int id)
        {
            var response = await _carService.RejectCarAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
