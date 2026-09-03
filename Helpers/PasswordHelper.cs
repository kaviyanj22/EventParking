using Event_parking.Models;
using Microsoft.AspNetCore.Identity;

namespace Event_parking.Helpers
{
    public class PasswordHelper
    {
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public string HashPassword(
            Customer customer,
            string password)
        {
            return _passwordHasher.HashPassword(
                customer,
                password
            );
        }

        public bool VerifyPassword(
            Customer customer,
            string passwordHash,
            string password)
        {
            var result =
                _passwordHasher.VerifyHashedPassword(
                    customer,
                    passwordHash,
                    password
                );

            return result != PasswordVerificationResult.Failed;
        }
    }
}