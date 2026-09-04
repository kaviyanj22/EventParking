using Event_parking.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event_parking.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        // ======================================
        // BOOKING
        // ======================================

        Task<Booking?> GetBookingWithDetailsAsync(
            int bookingId
        );

        // ======================================
        // PAYMENT
        // ======================================

        Task<Payment?> GetPaymentByBookingIdAsync(
            int bookingId
        );

        Task<Payment?> GetPaymentByIdAsync(
            int paymentId
        );

        Task<List<Payment>> GetPaymentsByCustomerAsync(
            int customerId
        );

        Task AddPaymentAsync(
            Payment payment
        );

        // ======================================
        // TRANSACTION / SAVE
        // ======================================

        Task<IDbContextTransaction>
            BeginTransactionAsync();

        Task<bool> SaveChangesAsync();
    }
}