using Event_parking.DTOs.Event;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/events
        // GET: api/events?name=Music
        // GET: api/events?date=2026-12-20
        // GET: api/events?venueId=1&categoryId=2
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] EventFilterDto filter)
        {
            var events =
                await _eventService.GetAllAsync(filter);

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eventResult =
                await _eventService.GetByIdAsync(id);

            if (eventResult == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(eventResult);
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] EventCreateDto createDto)
        {
            try
            {
                var eventResult =
                    await _eventService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = eventResult.EventId },
                    eventResult);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/events/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] EventUpdateDto updateDto)
        {
            try
            {
                var eventResult =
                    await _eventService.UpdateAsync(
                        id,
                        updateDto);

                if (eventResult == null)
                {
                    return NotFound(new
                    {
                        message = "Event not found."
                    });
                }

                return Ok(eventResult);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/events/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _eventService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Event not found."
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