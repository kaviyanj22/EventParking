using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);

        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer?>
            GetByVerificationTokenHashAsync(
                string tokenHash
            );

        Task<Customer?>
            GetByResetTokenHashAsync(
                string tokenHash
            );

        Task<List<Customer>> SearchAsync(
            string? search
        );

        Task AddAsync(Customer customer);

        Task SaveChangesAsync();
    }
}