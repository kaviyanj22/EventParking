using Event_parking.DTOs.Venue;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        // GET: api/venues
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venues = await _venueService.GetAllAsync();

            return Ok(venues);
        }

        // GET: api/venues/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);

            if (venue == null)
            {
                return NotFound(new
                {
                    message = "Venue not found."
                });
            }

            return Ok(venue);
        }

        // GET:
        // api/venues/available?date=2026-12-20
        // &startTime=18:00:00&endTime=22:00:00
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable(
            [FromQuery] DateTime date,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime,
            [FromQuery] int? venueId = null)
        {
            try
            {
                if (venueId.HasValue)
                {
                    var isAvailable =
                        await _venueService.IsAvailableAsync(
                            venueId.Value,
                            date,
                            startTime,
                            endTime);

                    return Ok(new
                    {
                        venueId = venueId.Value,
                        date,
                        startTime,
                        endTime,
                        isAvailable
                    });
                }

                var venues =
                    await _venueService.GetAvailableVenuesAsync(
                        date,
                        startTime,
                        endTime);

                return Ok(venues);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: api/venues
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] VenueCreateDto createDto)
        {
            var venue =
                await _venueService.CreateAsync(createDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = venue.VenueId },
                venue);
        }

        // PUT: api/venues/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] VenueUpdateDto updateDto)
        {
            var venue =
                await _venueService.UpdateAsync(
                    id,
                    updateDto);

            if (venue == null)
            {
                return NotFound(new
                {
                    message = "Venue not found."
                });
            }

            return Ok(venue);
        }

        // DELETE: api/venues/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _venueService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Venue not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}