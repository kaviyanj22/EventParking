using System.Security.Claims;
using Event_parking.DTOs.Customer;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService
            _customerService;

        public CustomerController(
            ICustomerService customerService)
        {
            _customerService = customerService;
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

        // GET: /api/customers/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<CustomerResponseDto> result =
                await _customerService
                    .GetCustomerAsync(customerId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // PUT: /api/customers/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody]
            CustomerUpdateDto updateDto)
        {
            int customerId =
                GetCurrentCustomerId();

            ServiceResult<CustomerResponseDto> result =
                await _customerService
                    .UpdateCustomerAsync(
                        customerId,
                        updateDto
                    );

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // GET: /api/customers?search=kavi
        // Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchCustomers(
            [FromQuery] string? search)
        {
            List<CustomerResponseDto> customers =
                await _customerService
                    .SearchCustomersAsync(search);

            return Ok(customers);
        }

        // GET: /api/customers/1
        // Admin only
        [HttpGet("{customerId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomer(
            int customerId)
        {
            ServiceResult<CustomerResponseDto> result =
                await _customerService
                    .GetCustomerAsync(customerId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // PUT: /api/customers/1/deactivate
        // Admin only
        [HttpPut("{customerId:int}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            DeactivateCustomer(int customerId)
        {
            ServiceResult<object> result =
                await _customerService
                    .ChangeCustomerStatusAsync(
                        customerId,
                        false
                    );

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // PUT: /api/customers/1/reactivate
        // Admin only
        [HttpPut("{customerId:int}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            ReactivateCustomer(int customerId)
        {
            ServiceResult<object> result =
                await _customerService
                    .ChangeCustomerStatusAsync(
                        customerId,
                        true
                    );

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}