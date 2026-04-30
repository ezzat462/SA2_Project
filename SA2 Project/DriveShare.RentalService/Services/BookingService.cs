using DriveShare.Shared.Models.Enums;
using DriveShare.Shared.DTOs.Common;
using DriveShare.RentalService.Data;
using DriveShare.RentalService.DTOs.Booking;
using DriveShare.RentalService.Models;
using DriveShare.RentalService.Services.Interfaces;
using DriveShare.Shared.Kafka;
using DriveShare.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.RentalService.Services
{
    public class BookingService : IBookingService
    {
        private readonly RentalDbContext _context;
        private readonly IKafkaProducer _kafkaProducer;

        public BookingService(RentalDbContext context, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        // ── Internal helper: auto-complete past bookings ─────────
        private async Task CheckAndCompletePastBookingsAsync()
        {
            var now = DateTime.UtcNow;
            var pastBookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.Status == BookingStatus.Accepted && b.EndDate < now)
                .ToListAsync();

            if (pastBookings.Any())
            {
                foreach (var b in pastBookings)
                {
                    b.Status = BookingStatus.Completed;
                    
                    // Check if there are any other ACTIVE accepted bookings for this car
                    var hasOtherActive = await _context.Bookings.AnyAsync(ob => 
                        ob.CarPostId == b.CarPostId && 
                        ob.Status == BookingStatus.Accepted && 
                        ob.StartDate <= now && ob.EndDate >= now);
                    
                    if (!hasOtherActive && b.Car != null)
                    {
                        b.Car.Status = CarStatus.Available;
                    }
                }
                await _context.SaveChangesAsync();
            }
        }

        // ───────────────────────────────────────────────────────────
        //  WORKFLOW 3A: Booking Request
        //  ► Validates availability and overlapping dates
        //  ► TODO: Publish Kafka "BookingConfirmed" event
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<BookingDto>> CreateBookingAsync(BookingRequestDto request, int renterId)
        {
            // Guard: Car must exist and be approved
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == request.CarPostId);

            if (car == null)
                return ApiResponse<BookingDto>.FailureResponse("Car not found.");

            if (!car.IsApproved)
                return ApiResponse<BookingDto>.FailureResponse("This car listing is not yet approved.");

            // Guard: Dates must fall within the car's availability window
            if (request.StartDate < car.AvailableFrom || request.EndDate > car.AvailableTo)
            {
                return ApiResponse<BookingDto>.FailureResponse("Requested dates fall outside the car's availability window.");
            }

            // Guard: No overlapping accepted bookings
            var overlapping = await _context.Bookings.AnyAsync(b =>
                b.CarPostId == request.CarPostId &&
                b.Status == BookingStatus.Accepted &&
                ((request.StartDate >= b.StartDate && request.StartDate <= b.EndDate) ||
                 (request.EndDate >= b.StartDate && request.EndDate <= b.EndDate) ||
                 (request.StartDate <= b.StartDate && request.EndDate >= b.EndDate)));

            if (overlapping)
            {
                return ApiResponse<BookingDto>.FailureResponse("The car is already booked for the selected dates.");
            }

            // Calculate pricing
            var totalDays = (int)(request.EndDate - request.StartDate).TotalDays;
            if (totalDays <= 0) totalDays = 1;

            var booking = new Booking
            {
                CarPostId = request.CarPostId,
                RenterId = renterId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = totalDays * car.PricePerDay,
                Status = BookingStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // TODO: Publish Kafka "NewBookingRequest" event for NotificationService

            var dto = new BookingDto
            {
                Id = booking.Id,
                CarPostId = booking.CarPostId,
                CarBrand = car.Brand,
                CarModel = car.Model,
                RenterId = booking.RenterId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status.ToString(),
                RequestedAt = booking.RequestedAt
            };

            return ApiResponse<BookingDto>.SuccessResponse(dto, "Booking request sent successfully.");
        }

        // ───────────────────────────────────────────────────────────
        //  WORKFLOW 3B: Owner Accepts Booking
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<bool>> AcceptBookingAsync(int bookingId, int ownerId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Car == null || booking.Car.OwnerId != ownerId)
            {
                return ApiResponse<bool>.FailureResponse("Booking not found or access denied.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return ApiResponse<bool>.FailureResponse("This booking has already been resolved.");
            }

            booking.Status = BookingStatus.Accepted;
            booking.RespondedAt = DateTime.UtcNow;

            // If the booking is active right now, mark car as Rented
            var now = DateTime.UtcNow;
            if (booking.StartDate <= now && booking.EndDate >= now)
            {
                booking.Car.Status = CarStatus.Rented;
            }
            
            await _context.SaveChangesAsync();

            await _kafkaProducer.ProduceAsync("booking-confirmed-topic", new BookingConfirmedEvent
            {
                BookingId = booking.Id,
                UserId = booking.RenterId.ToString(),
                CarId = booking.CarPostId
            });

            return ApiResponse<bool>.SuccessResponse(true, "Booking accepted.");
        }

        // ───────────────────────────────────────────────────────────
        //  WORKFLOW 3B: Owner Rejects Booking
        // ───────────────────────────────────────────────────────────
        public async Task<ApiResponse<bool>> RejectBookingAsync(int bookingId, int ownerId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Car == null || booking.Car.OwnerId != ownerId)
            {
                return ApiResponse<bool>.FailureResponse("Booking not found or access denied.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return ApiResponse<bool>.FailureResponse("This booking has already been resolved.");
            }

            booking.Status = BookingStatus.Rejected;
            booking.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // TODO: Publish Kafka "BookingRejected" event for NotificationService

            return ApiResponse<bool>.SuccessResponse(true, "Booking rejected.");
        }

        public async Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync(int renterId)
        {
            await CheckAndCompletePastBookingsAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.RenterId == renterId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    CarPostId = b.CarPostId,
                    CarBrand = b.Car!.Brand,
                    CarModel = b.Car.Model,
                    RenterId = b.RenterId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status.ToString(),
                    RequestedAt = b.RequestedAt,
                    RespondedAt = b.RespondedAt
                })
                .OrderByDescending(b => b.RequestedAt)
                .ToListAsync();

            return ApiResponse<List<BookingDto>>.SuccessResponse(bookings);
        }

        public async Task<ApiResponse<List<BookingDto>>> GetIncomingBookingsAsync(int ownerId)
        {
            await CheckAndCompletePastBookingsAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.Car!.OwnerId == ownerId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    CarPostId = b.CarPostId,
                    CarBrand = b.Car!.Brand,
                    CarModel = b.Car.Model,
                    RenterId = b.RenterId,
                    RenterName = "User #" + b.RenterId, // TODO: Fetch from UserService via HTTP/gRPC
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status.ToString(),
                    RequestedAt = b.RequestedAt,
                    RespondedAt = b.RespondedAt
                })
                .OrderByDescending(b => b.RequestedAt)
                .ToListAsync();

            return ApiResponse<List<BookingDto>>.SuccessResponse(bookings);
        }
    }
}
