using Event_parking.DTOs.Customer;

namespace Event_parking.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ServiceResult<CustomerResponseDto>>
            GetCustomerAsync(
                int customerId
            );

        Task<ServiceResult<CustomerResponseDto>>
            UpdateCustomerAsync(
                int customerId,
                CustomerUpdateDto updateDto
            );

        Task<List<CustomerResponseDto>>
            SearchCustomersAsync(
                string? search
            );

        Task<ServiceResult<object>>
            ChangeCustomerStatusAsync(
                int customerId,
                bool activate
            );
    }
}