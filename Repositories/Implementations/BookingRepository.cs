using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Event_parking.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // CUSTOMER
        // ======================================

        public async Task<Customer?> GetCustomerByIdAsync(
            int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(customer =>
                    customer.CustomerId == customerId);
        }

        // ======================================
        // EVENT
        // ======================================

        public async Task<Event?> GetEventByIdAsync(
            int eventId)
        {
            return await _context.Events
                .FirstOrDefaultAsync(eventItem =>
                    eventItem.EventId == eventId);
        }

        // ======================================
        // SEATS
        // ======================================

        public async Task<List<Seat>> GetSeatsByIdsAsync(
            int eventId,
            IEnumerable<int> seatIds)
        {
            List<int> ids = seatIds
                .Distinct()
                .ToList();

            return await _context.Seats
                .Where(seat =>
                    seat.EventId == eventId &&
                    ids.Contains(seat.SeatId))
                .ToListAsync();
        }

        public async Task<bool> HasActiveBookingSeatAsync(
            int seatId)
        {
            return await _context.BookingSeats
                .AnyAsync(bookingSeat =>
                    bookingSeat.SeatId == seatId &&
                    bookingSeat.IsActive);
        }

        // ======================================
        // PARKING
        // ======================================

        public async Task<ParkingSlot?> GetParkingSlotByIdAsync(
            int eventId,
            int parkingSlotId)
        {
            return await _context.ParkingSlots
                .FirstOrDefaultAsync(parkingSlot =>
                    parkingSlot.EventId == eventId &&
                    parkingSlot.ParkingSlotId == parkingSlotId);
        }

        public async Task<bool>
            HasActiveParkingReservationAsync(
                int parkingSlotId)
        {
            return await _context.ParkingReservations
                .AnyAsync(reservation =>
                    reservation.ParkingSlotId == parkingSlotId &&
                    reservation.IsActive);
        }

        // ======================================
        // BOOKING NUMBER
        // ======================================

        public async Task<bool> BookingNumberExistsAsync(
            string bookingNumber)
        {
            return await _context.Bookings
                .AnyAsync(booking =>
                    booking.BookingNumber == bookingNumber);
        }

        // ======================================
        // ADD BOOKING
        // ======================================

        public async Task AddBookingAsync(
            Booking booking)
        {
            await _context.Bookings
                .AddAsync(booking);
        }

        // ======================================
        // GET ONE BOOKING
        // ======================================

        public async Task<Booking?> GetBookingWithDetailsAsync(
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
        // CUSTOMER BOOKINGS
        // ======================================

        public async Task<List<Booking>>
            GetBookingsByCustomerAsync(
                int customerId)
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

                .Where(booking =>
                    booking.CustomerId == customerId)

                .OrderByDescending(booking =>
                    booking.CreatedAt)

                .ToListAsync();
        }

        // ======================================
        // ADMIN BOOKING LIST
        // ======================================

        public async Task<List<Booking>>
            GetBookingsAsync(
                int? eventId)
        {
            IQueryable<Booking> query =
                _context.Bookings

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
                        booking.Payment);

            if (eventId.HasValue)
            {
                query = query.Where(booking =>
                    booking.EventId == eventId.Value);
            }

            return await query
                .OrderByDescending(booking =>
                    booking.CreatedAt)
                .ToListAsync();
        }

        // ======================================
        // EXPIRED PENDING BOOKINGS
        // ======================================

        public async Task<List<Booking>>
            GetExpiredPendingBookingsAsync(
                DateTime utcNow)
        {
            return await _context.Bookings

                .Include(booking =>
                    booking.BookingSeats)
                    .ThenInclude(bookingSeat =>
                        bookingSeat.Seat)

                .Include(booking =>
                    booking.ParkingReservation)
                    .ThenInclude(reservation =>
                        reservation!.ParkingSlot)

                .Where(booking =>
                    booking.Status == "Pending" &&
                    booking.HoldExpiresAt != null &&
                    booking.HoldExpiresAt <= utcNow)

                .ToListAsync();
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