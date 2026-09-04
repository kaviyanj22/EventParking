using Event_parking.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event_parking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        // ======================================
        // CUSTOMER / EVENT
        // ======================================

        Task<Customer?> GetCustomerByIdAsync(
            int customerId
        );

        Task<Event?> GetEventByIdAsync(
            int eventId
        );

        // ======================================
        // SEATS
        // ======================================

        Task<List<Seat>> GetSeatsByIdsAsync(
            int eventId,
            IEnumerable<int> seatIds
        );

        Task<bool> HasActiveBookingSeatAsync(
            int seatId
        );

        // ======================================
        // PARKING
        // ======================================

        Task<ParkingSlot?> GetParkingSlotByIdAsync(
            int eventId,
            int parkingSlotId
        );

        Task<bool> HasActiveParkingReservationAsync(
            int parkingSlotId
        );

        // ======================================
        // BOOKING
        // ======================================

        Task<bool> BookingNumberExistsAsync(
            string bookingNumber
        );

        Task AddBookingAsync(
            Booking booking
        );

        Task<Booking?> GetBookingWithDetailsAsync(
            int bookingId
        );

        Task<List<Booking>> GetBookingsByCustomerAsync(
            int customerId
        );

        Task<List<Booking>> GetBookingsAsync(
            int? eventId
        );

        // ======================================
        // EXPIRED BOOKINGS
        // ======================================

        Task<List<Booking>> GetExpiredPendingBookingsAsync(
            DateTime utcNow
        );

        // ======================================
        // TRANSACTION / SAVE
        // ======================================

        Task<IDbContextTransaction>
            BeginTransactionAsync();

        Task<bool> SaveChangesAsync();
    }
}