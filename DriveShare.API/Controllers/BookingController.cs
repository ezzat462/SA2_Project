using DriveShare.API.DTOs.Booking;
using DriveShare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "Renter")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _bookingService.CreateBookingAsync(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptBooking(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _bookingService.AcceptBookingAsync(id, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/reject")]
        [Authorize]
        public async Task<IActionResult> RejectBooking(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _bookingService.RejectBookingAsync(id, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Renter")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _bookingService.GetMyBookingsAsync(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("incoming")]
        [Authorize]
        public async Task<IActionResult> GetIncomingBookings()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var response = await _bookingService.GetIncomingBookingsAsync(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
