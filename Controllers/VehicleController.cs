using System.Security.Claims;
using Event_parking.DTOs.Vehicle;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService
            _vehicleService;

        public VehicleController(
            IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        private int GetCurrentCustomerId()
        {
            string? customerId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (
                string.IsNullOrWhiteSpace(customerId)
                ||
                !int.TryParse(
                    customerId,
                    out int parsedCustomerId)
            )
            {
                throw new UnauthorizedAccessException(
                    "Invalid authentication token."
                );
            }

            return parsedCustomerId;
        }

        // GET: /api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetMyVehicles()
        {
            int customerId =
                GetCurrentCustomerId();

            List<VehicleResponseDto> vehicles =
                await _vehicleService
                    .GetMyVehiclesAsync(customerId);

            return Ok(vehicles);
        }

        // POST: /api/vehicles
        [HttpPost]
        public async Task<IActionResult> AddVehicle(
            [FromBody]
            VehicleCreateDto createDto)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<VehicleResponseDto> result =
                await _vehicleService
                    .CreateVehicleAsync(
                        customerId,
                        createDto
                    );

            if (!result.Success)
            {
                return Conflict(result);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }

        // PUT: /api/vehicles/1
        [HttpPut("{vehicleId:int}")]
        public async Task<IActionResult> UpdateVehicle(
            int vehicleId,
            [FromBody]
            VehicleUpdateDto updateDto)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<VehicleResponseDto> result =
                await _vehicleService
                    .UpdateVehicleAsync(
                        customerId,
                        vehicleId,
                        updateDto
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: /api/vehicles/1
        [HttpDelete("{vehicleId:int}")]
        public async Task<IActionResult> DeleteVehicle(
            int vehicleId)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<object> result =
                await _vehicleService
                    .DeleteVehicleAsync(
                        customerId,
                        vehicleId
                    );

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}