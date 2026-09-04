using System.Security.Claims;
using Event_parking.DTOs.Notification;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService
            _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService =
                notificationService;
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
        // GET /api/notifications/customer/{customerId}
        // ======================================

        [HttpGet("customer/{customerId:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetCustomerNotifications(
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
                        "You are not authorized to access these notifications."));
            }

            ServiceResult<
                List<NotificationResponseDto>> result =
                    await _notificationService
                        .GetCustomerNotificationsAsync(
                            customerId);

            return Ok(result);
        }

        // ======================================
        // PUT /api/notifications/{id}/read
        // ======================================

        [HttpPut("{id:int}/read")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            MarkAsRead(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<NotificationResponseDto> result =
                await _notificationService
                    .MarkAsReadAsync(
                        id,
                        customerId);

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
    }
}