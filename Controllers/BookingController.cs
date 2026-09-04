using System.Security.Claims;
using Event_parking.DTOs.Booking;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(
            IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ======================================
        // GET CURRENT CUSTOMER ID FROM JWT
        // ======================================

        private int GetCurrentCustomerId()
        {
            string? customerId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(customerId) ||
                !int.TryParse(
                    customerId,
                    out int parsedCustomerId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid authentication token.");
            }

            return parsedCustomerId;
        }

        // ======================================
        // POST /api/bookings
        // CREATE BOOKING
        // ======================================

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            CreateBooking(
                [FromBody] BookingCreateDto dto)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<BookingResponseDto> result =
                await _bookingService
                    .CreateBookingAsync(
                        customerId,
                        dto);

            if (!result.Success)
            {
                return MapBookingError(result);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }

        // ======================================
        // GET /api/bookings/customer/{customerId}
        // ======================================

        [HttpGet("customer/{customerId:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetCustomerBookings(
                int customerId)
        {
            int currentCustomerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            if (!isAdmin &&
                currentCustomerId != customerId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ServiceResult<object>.Fail(
                        "You are not authorized to access these bookings."));
            }

            ServiceResult<List<BookingResponseDto>>
                result =
                    await _bookingService
                        .GetCustomerBookingsAsync(
                            customerId);

            return Ok(result);
        }

        // ======================================
        // GET /api/bookings/{id}
        // ======================================

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetBookingById(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            ServiceResult<BookingResponseDto> result =
                await _bookingService
                    .GetBookingByIdAsync(
                        id,
                        customerId,
                        isAdmin);

            if (!result.Success)
            {
                return MapBookingError(result);
            }

            return Ok(result);
        }

        // ======================================
        // GET /api/bookings/{id}/hold-status
        // ======================================

        [HttpGet("{id:int}/hold-status")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetHoldStatus(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            ServiceResult<BookingHoldStatusDto> result =
                await _bookingService
                    .GetHoldStatusAsync(
                        id,
                        customerId,
                        isAdmin);

            if (!result.Success)
            {
                if (result.Message.Contains(
                    "not authorized",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        result);
                }

                if (result.Message.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        // ======================================
        // DELETE /api/bookings/{id}
        // CANCEL BOOKING
        // ======================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            CancelBooking(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            ServiceResult<bool> result =
                await _bookingService
                    .CancelBookingAsync(
                        id,
                        customerId,
                        isAdmin);

            if (!result.Success)
            {
                if (result.Message.Contains(
                    "not authorized",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        result);
                }

                if (result.Message.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        // ======================================
        // GET /api/bookings?eventId=1
        // ADMIN ONLY
        // ======================================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            GetBookings(
                [FromQuery] int? eventId)
        {
            ServiceResult<List<BookingResponseDto>>
                result =
                    await _bookingService
                        .GetBookingsAsync(
                            eventId);

            return Ok(result);
        }

        // ======================================
        // COMMON ERROR RESPONSE
        // ======================================

        private IActionResult MapBookingError(
            ServiceResult<BookingResponseDto> result)
        {
            if (result.Message.Contains(
                    "not authorized",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "verify your email",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "account is not active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    result);
            }

            if (result.Message.Contains(
                    "not available",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "already been booked",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "already been reserved",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(result);
            }

            if (result.Message.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "do not exist",
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }
    }
}