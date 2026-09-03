using Event_parking.DTOs.Customer;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository
            _customerRepository;

        public CustomerService(
            ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<
            ServiceResult<CustomerResponseDto>>
            GetCustomerAsync(int customerId)
        {
            Customer? customer =
                await _customerRepository
                    .GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<CustomerResponseDto>
                    .Fail("Customer not found.");
            }

            return ServiceResult<CustomerResponseDto>
                .Ok(MapCustomer(customer));
        }

        public async Task<
            ServiceResult<CustomerResponseDto>>
            UpdateCustomerAsync(
                int customerId,
                CustomerUpdateDto updateDto)
        {
            Customer? customer =
                await _customerRepository
                    .GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<CustomerResponseDto>
                    .Fail("Customer not found.");
            }

            customer.FullName =
                updateDto.FullName.Trim();

            customer.Phone =
                updateDto.Phone.Trim();

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.SaveChangesAsync();

            return ServiceResult<CustomerResponseDto>
                .Ok(
                    MapCustomer(customer),
                    "Customer profile updated successfully."
                );
        }

        public async Task<List<CustomerResponseDto>>
            SearchCustomersAsync(string? search)
        {
            List<Customer> customers =
                await _customerRepository
                    .SearchAsync(search);

            return customers
                .Select(MapCustomer)
                .ToList();
        }

        public async Task<ServiceResult<object>>
            ChangeCustomerStatusAsync(
                int customerId,
                bool activate)
        {
            Customer? customer =
                await _customerRepository
                    .GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<object>
                    .Fail("Customer not found.");
            }

            customer.Status =
                activate
                    ? "Active"
                    : "Deactivated";

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.SaveChangesAsync();

            string message = activate
                ? "Customer account reactivated."
                : "Customer account deactivated.";

            return ServiceResult<object>.Ok(
                null,
                message
            );
        }

        private static CustomerResponseDto MapCustomer(
            Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Role = customer.Role,
                Status = customer.Status,
                EmailVerified =
                    customer.EmailVerified,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}