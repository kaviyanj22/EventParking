using Event_parking.DTOs.SeatSection;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/seat-sections")]
    public class SeatSectionController : ControllerBase
    {
        private readonly ISeatSectionService
            _seatSectionService;

        public SeatSectionController(
            ISeatSectionService seatSectionService)
        {
            _seatSectionService =
                seatSectionService;
        }

        // ==========================================
        // GET ALL SECTIONS FOR EVENT
        // GET: /api/events/{eventId}/seat-sections
        // PUBLIC
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetSections(
            int eventId)
        {
            var result =
                await _seatSectionService
                    .GetByEventIdAsync(eventId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // ==========================================
        // GET SINGLE SECTION
        // GET: /api/events/{eventId}/seat-sections/{id}
        // PUBLIC
        // ==========================================
        [HttpGet("{seatSectionId:int}")]
        public async Task<IActionResult> GetSection(
            int eventId,
            int seatSectionId)
        {
            var result =
                await _seatSectionService
                    .GetByIdAsync(
                        eventId,
                        seatSectionId
                    );

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // ==========================================
        // CREATE SECTION
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSection(
            int eventId,
            [FromBody] SeatSectionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _seatSectionService
                    .CreateAsync(
                        eventId,
                        dto
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }

        // ==========================================
        // UPDATE SECTION
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{seatSectionId:int}")]
        public async Task<IActionResult> UpdateSection(
            int eventId,
            int seatSectionId,
            [FromBody] SeatSectionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await _seatSectionService
                    .UpdateAsync(
                        eventId,
                        seatSectionId,
                        dto
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // ==========================================
        // DELETE SECTION
        // ADMIN ONLY
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{seatSectionId:int}")]
        public async Task<IActionResult> DeleteSection(
            int eventId,
            int seatSectionId)
        {
            var result =
                await _seatSectionService
                    .DeleteAsync(
                        eventId,
                        seatSectionId
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}