using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class CustomerRepository
        : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(
            int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(
                    customer =>
                        customer.CustomerId == customerId
                );
        }

        public async Task<Customer?> GetByEmailAsync(
            string email)
        {
            string normalizedEmail =
                email.Trim().ToLower();

            return await _context.Customers
                .FirstOrDefaultAsync(
                    customer =>
                        customer.Email == normalizedEmail
                );
        }

        public async Task<Customer?>
            GetByVerificationTokenHashAsync(
                string tokenHash)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(
                    customer =>
                        customer.EmailVerificationTokenHash
                        == tokenHash
                );
        }

        public async Task<Customer?>
            GetByResetTokenHashAsync(
                string tokenHash)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(
                    customer =>
                        customer.PasswordResetTokenHash
                        == tokenHash
                );
        }

        public async Task<List<Customer>> SearchAsync(
            string? search)
        {
            IQueryable<Customer> query =
                _context.Customers
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchValue = search.Trim();

                query = query.Where(
                    customer =>
                        customer.FullName.Contains(searchValue)
                        ||
                        customer.Email.Contains(searchValue)
                );
            }

            return await query
                .OrderBy(customer => customer.FullName)
                .ToListAsync();
        }

        public async Task AddAsync(
            Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}