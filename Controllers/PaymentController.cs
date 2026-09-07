using System.Security.Claims;
using Event_parking.DTOs.Payment;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService
            _paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService =
                paymentService;
        }

        // ======================================
        // GET CURRENT CUSTOMER ID FROM JWT
        // ======================================

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
                    out int parsedCustomerId
                )
            )
            {
                throw new UnauthorizedAccessException(
                    "Invalid authentication token."
                );
            }

            return parsedCustomerId;
        }

        // ======================================
        // GET PAYMENT SUMMARY
        // GET /api/bookings/{id}/payment
        //
        // Works BEFORE and AFTER payment.
        // Returns:
        // Seat Total
        // Parking Fee
        // Total Amount
        // Booking Status
        // Payment Status
        // Hold Expiry
        // Remaining Seconds
        // ======================================

        [HttpGet("bookings/{id:int}/payment")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetPaymentSummary(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            ServiceResult<PaymentSummaryDto> result =
                await _paymentService
                    .GetPaymentSummaryAsync(
                        id,
                        customerId,
                        isAdmin
                    );

            if (!result.Success)
            {
                return MapPaymentSummaryError(
                    result
                );
            }

            return Ok(result);
        }

        // ======================================
        // POST PAYMENT
        // POST /api/bookings/{id}/payment
        // CUSTOMER ONLY
        // ======================================

        [HttpPost("bookings/{id:int}/payment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            CreatePayment(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<PaymentResponseDto> result =
                await _paymentService
                    .CreatePaymentAsync(
                        id,
                        customerId
                    );

            if (!result.Success)
            {
                return MapPaymentError(
                    result
                );
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }



        // ======================================
        // GET ALL PAYMENTS
        // GET /api/payments
        // ADMIN ONLY
        // ======================================

        [HttpGet("payments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            GetAllPayments()
        {
            ServiceResult<List<PaymentHistoryDto>> result =
                await _paymentService
                    .GetAllPaymentsAsync();

            return Ok(result);
        }


        // ======================================
        // CUSTOMER PAYMENT HISTORY
        // GET /api/payments/customer/{customerId}
        // ======================================

        [HttpGet(
            "payments/customer/{customerId:int}"
        )]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetCustomerPayments(
                int customerId)
        {
            int currentCustomerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            if (
                !isAdmin
                &&
                currentCustomerId != customerId
            )
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ServiceResult<object>.Fail(
                        "You are not authorized to access these payments."
                    )
                );
            }

            ServiceResult<
                List<PaymentHistoryDto>> result =
                    await _paymentService
                        .GetCustomerPaymentsAsync(
                            customerId
                        );

            return Ok(result);
        }

        // ======================================
        // GET RECEIPT
        // GET /api/payments/{id}/receipt
        // ======================================

        [HttpGet(
            "payments/{id:int}/receipt"
        )]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult>
            GetReceipt(
                int id)
        {
            int customerId =
                GetCurrentCustomerId();

            bool isAdmin =
                User.IsInRole("Admin");

            ServiceResult<ReceiptDto> result =
                await _paymentService
                    .GetReceiptAsync(
                        id,
                        customerId,
                        isAdmin
                    );

            if (!result.Success)
            {
                if (
                    result.Message.Contains(
                        "not authorized",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        result
                    );
                }

                if (
                    result.Message.Contains(
                        "not found",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            return Ok(result);
        }

        // ======================================
        // PAYMENT SUMMARY ERROR RESPONSE
        // ======================================

        private IActionResult
            MapPaymentSummaryError(
                ServiceResult<PaymentSummaryDto>
                    result)
        {
            if (
                result.Message.Contains(
                    "not authorized",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    result
                );
            }

            if (
                result.Message.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        // ======================================
        // PAYMENT ERROR RESPONSE
        // ======================================

        private IActionResult MapPaymentError(
            ServiceResult<PaymentResponseDto>
                result)
        {
            if (
                result.Message.Contains(
                    "not authorized",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    result
                );
            }

            if (
                result.Message.Contains(
                    "not found",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return NotFound(result);
            }

            if (
                result.Message.Contains(
                    "already exists",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Conflict(result);
            }

            return BadRequest(result);
        }
    }
}