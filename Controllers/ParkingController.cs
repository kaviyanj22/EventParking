using Event_parking.DTOs.Parking;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/parking-slots")]
    public class ParkingController : ControllerBase
    {
        private readonly IParkingService _parkingService;

        public ParkingController(IParkingService parkingService)
        {
            _parkingService = parkingService;
        }

        // ==========================================
        // GET FULL PARKING LAYOUT
        // GET: /api/events/{eventId}/parking-slots
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetParkingSlots(
            int eventId)
        {
            var parkingSlots =
                await _parkingService
                    .GetParkingSlotsByEventIdAsync(eventId);

            return Ok(parkingSlots);
        }

        // ==========================================
        // GET SINGLE PARKING SLOT
        // GET: /api/events/{eventId}/parking-slots/{slotId}
        // ==========================================
        [HttpGet("{slotId:int}")]
        public async Task<IActionResult> GetParkingSlot(
            int eventId,
            int slotId)
        {
            var parkingSlot =
                await _parkingService
                    .GetParkingSlotByIdAsync(
                        eventId,
                        slotId
                    );

            if (parkingSlot == null)
            {
                return NotFound(new
                {
                    message = "Parking slot not found."
                });
            }

            return Ok(parkingSlot);
        }

        // ==========================================
        // CREATE FULL PARKING LAYOUT
        // POST: /api/events/{eventId}/parking-slots
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateParkingLayout(
            int eventId,
            [FromBody] ParkingLayoutCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _parkingService
                    .CreateParkingLayoutAsync(
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
        // CREATE SINGLE PARKING SLOT
        // POST: /api/events/{eventId}/parking-slots/single
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost("single")]
        public async Task<IActionResult> CreateParkingSlot(
            int eventId,
            [FromBody] ParkingSlotCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _parkingService
                    .CreateParkingSlotAsync(
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
        // UPDATE PARKING SLOT
        // PUT: /api/events/{eventId}/parking-slots/{slotId}
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{slotId:int}")]
        public async Task<IActionResult> UpdateParkingSlot(
            int eventId,
            int slotId,
            [FromBody] ParkingSlotUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _parkingService
                    .UpdateParkingSlotAsync(
                        eventId,
                        slotId,
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
        // DELETE PARKING SLOT
        // DELETE: /api/events/{eventId}/parking-slots/{slotId}
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{slotId:int}")]
        public async Task<IActionResult> DeleteParkingSlot(
            int eventId,
            int slotId)
        {
            var result =
                await _parkingService
                    .DeleteParkingSlotAsync(
                        eventId,
                        slotId
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