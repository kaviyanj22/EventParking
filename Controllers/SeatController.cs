using Event_parking.DTOs.Seat;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/seats")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // ==========================================
        // GET FULL SEAT MAP
        // GET: /api/events/{eventId}/seats
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetSeats(
            int eventId)
        {
            var seats =
                await _seatService
                    .GetSeatsByEventIdAsync(eventId);

            return Ok(seats);
        }

        // ==========================================
        // GET SINGLE SEAT
        // GET: /api/events/{eventId}/seats/{seatId}
        // ==========================================
        [HttpGet("{seatId:int}")]
        public async Task<IActionResult> GetSeat(
            int eventId,
            int seatId)
        {
            var seat =
                await _seatService
                    .GetSeatByIdAsync(
                        eventId,
                        seatId
                    );

            if (seat == null)
            {
                return NotFound(new
                {
                    message = "Seat not found."
                });
            }

            return Ok(seat);
        }

        // ==========================================
        // CREATE FULL SEAT MAP
        // POST: /api/events/{eventId}/seats
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSeatMap(
            int eventId,
            [FromBody] SeatMapCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _seatService
                    .CreateSeatMapAsync(
                        eventId,
                        dto
                    );

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = result.Message
                }
            );
        }

        // ==========================================
        // CREATE SINGLE SEAT
        // POST: /api/events/{eventId}/seats/single
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost("single")]
        public async Task<IActionResult> CreateSeat(
            int eventId,
            [FromBody] SeatCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _seatService
                    .CreateSeatAsync(
                        eventId,
                        dto
                    );

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = result.Message,
                    data = result.Data
                }
            );
        }

        // ==========================================
        // UPDATE SEAT
        // PUT: /api/events/{eventId}/seats/{seatId}
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{seatId:int}")]
        public async Task<IActionResult> UpdateSeat(
            int eventId,
            int seatId,
            [FromBody] SeatUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _seatService
                    .UpdateSeatAsync(
                        eventId,
                        seatId,
                        dto
                    );

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                message = result.Message
            });
        }

        // ==========================================
        // DELETE SEAT
        // DELETE: /api/events/{eventId}/seats/{seatId}
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{seatId:int}")]
        public async Task<IActionResult> DeleteSeat(
            int eventId,
            int seatId)
        {
            var result =
                await _seatService
                    .DeleteSeatAsync(
                        eventId,
                        seatId
                    );

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                message = result.Message
            });
        }
    }
}