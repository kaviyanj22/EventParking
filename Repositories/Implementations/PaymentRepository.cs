using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event_parking.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET BOOKING WITH DETAILS
        // ======================================

        public async Task<Booking?>
            GetBookingWithDetailsAsync(
                int bookingId)
        {
            return await _context.Bookings

                .Include(booking =>
                    booking.Customer)

                .Include(booking =>
                    booking.Event)

                .Include(booking =>
                    booking.BookingSeats)
                    .ThenInclude(bookingSeat =>
                        bookingSeat.Seat)

                .Include(booking =>
                    booking.ParkingReservation)
                    .ThenInclude(reservation =>
                        reservation!.ParkingSlot)

                .Include(booking =>
                    booking.Payment)

                .FirstOrDefaultAsync(booking =>
                    booking.BookingId == bookingId);
        }

        // ======================================
        // GET PAYMENT BY BOOKING
        // ======================================

        public async Task<Payment?>
            GetPaymentByBookingIdAsync(
                int bookingId)
        {
            return await _context.Payments

                .Include(payment =>
                    payment.Booking)

                .FirstOrDefaultAsync(payment =>
                    payment.BookingId == bookingId);
        }

        // ======================================
        // GET PAYMENT BY ID
        // ======================================

        public async Task<Payment?>
            GetPaymentByIdAsync(
                int paymentId)
        {
            return await _context.Payments

                .Include(payment =>
                    payment.Customer)

                .Include(payment =>
                    payment.Booking)
                    .ThenInclude(booking =>
                        booking!.Event)

                .Include(payment =>
                    payment.Booking)
                    .ThenInclude(booking =>
                        booking!.BookingSeats)

                .Include(payment =>
                    payment.Booking)
                    .ThenInclude(booking =>
                        booking!.ParkingReservation)

                .FirstOrDefaultAsync(payment =>
                    payment.PaymentId == paymentId);
        }

        // ======================================
        // CUSTOMER PAYMENT HISTORY
        // ======================================

        public async Task<List<Payment>>
            GetPaymentsByCustomerAsync(
                int customerId)
        {
            return await _context.Payments

                .Include(payment =>
                    payment.Booking)
                    .ThenInclude(booking =>
                        booking!.Event)

                .Where(payment =>
                    payment.CustomerId == customerId)

                .OrderByDescending(payment =>
                    payment.PaidAt)

                .ToListAsync();
        }

        // ======================================
        // ADD PAYMENT
        // ======================================

        public async Task AddPaymentAsync(
            Payment payment)
        {
            await _context.Payments
                .AddAsync(payment);
        }

        // ======================================
        // DATABASE TRANSACTION
        // ======================================

        public async Task<IDbContextTransaction>
            BeginTransactionAsync()
        {
            return await _context.Database
                .BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);
        }

        // ======================================
        // SAVE
        // ======================================

        public async Task<bool> SaveChangesAsync()
        {
            return await _context
                .SaveChangesAsync() > 0;
        }
    }
}