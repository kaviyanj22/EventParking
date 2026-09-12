using Event_parking.DTOs.Event;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _environment;

        public EventController(
            IEventService eventService,
            IWebHostEnvironment environment)
        {
            _eventService = eventService;
            _environment = environment;
        }

        // ==========================================
        // GET ALL EVENTS
        // PUBLIC
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] EventFilterDto filter)
        {
            var events =
                await _eventService
                    .GetAllAsync(filter);

            return Ok(events);
        }

        // ==========================================
        // GET EVENT BY ID
        // PUBLIC
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var eventResult =
                await _eventService
                    .GetByIdAsync(id);

            if (eventResult == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(eventResult);
        }

        // ==========================================
        // UPLOAD EVENT POSTER IMAGE
        // ADMIN ONLY
        // ==========================================
        // POST: api/events/upload-image
        // multipart/form-data
        // field name: file
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadEventImage(
            IFormFile file)
        {
            if (file == null ||
                file.Length == 0)
            {
                return BadRequest(new
                {
                    message =
                        "Please select an image."
                });
            }

            // Maximum 5 MB
            const long maxFileSize =
                5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                return BadRequest(new
                {
                    message =
                        "Image size cannot exceed 5 MB."
                });
            }

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    message =
                        "Only JPG, JPEG, PNG and WEBP images are allowed."
                });
            }

            if (string.IsNullOrWhiteSpace(
                    file.ContentType) ||
                !file.ContentType.StartsWith(
                    "image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "The selected file is not a valid image."
                });
            }

            string webRootPath =
                _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(
                    webRootPath))
            {
                webRootPath =
                    Path.Combine(
                        _environment.ContentRootPath,
                        "wwwroot"
                    );
            }

            string uploadFolder =
                Path.Combine(
                    webRootPath,
                    "uploads",
                    "events"
                );

            if (!Directory.Exists(
                    uploadFolder))
            {
                Directory.CreateDirectory(
                    uploadFolder
                );
            }

            string fileName =
                $"{Guid.NewGuid():N}{extension}";

            string filePath =
                Path.Combine(
                    uploadFolder,
                    fileName
                );

            await using (
                var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create
                    )
            )
            {
                await file.CopyToAsync(
                    stream
                );
            }

            string imageUrl =
                $"/uploads/events/{fileName}";

            return Ok(new
            {
                message =
                    "Event image uploaded successfully.",

                imageUrl
            });
        }

        // ==========================================
        // CREATE EVENT
        // ADMIN ONLY
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] EventCreateDto createDto)
        {
            try
            {
                var eventResult =
                    await _eventService
                        .CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = eventResult.EventId
                    },
                    eventResult
                );
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

        // ==========================================
        // UPDATE EVENT
        // ADMIN ONLY
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] EventUpdateDto updateDto)
        {
            try
            {
                var eventResult =
                    await _eventService
                        .UpdateAsync(
                            id,
                            updateDto
                        );

                if (eventResult == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Event not found."
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

        // ==========================================
        // DELETE EVENT
        // ADMIN ONLY
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                var deleted =
                    await _eventService
                        .DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message =
                            "Event not found."
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