using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetSeatsByEventIdAsync(
            int eventId)
        {
            return await _context.Seats
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.ColumnNumber)
                .ToListAsync();
        }

        public async Task<Seat?> GetSeatByIdAsync(int seatId)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatId == seatId);
        }

        public async Task<bool> EventExistsAsync(int eventId)
        {
            return await _context.Events
                .AnyAsync(e => e.EventId == eventId);
        }

        public async Task<int> GetEventCapacityAsync(int eventId)
        {
            return await _context.Events
                .Where(e => e.EventId == eventId)
                .Select(e => e.Capacity)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SeatNumberExistsAsync(
            int eventId,
            string seatNumber,
            int? excludeSeatId = null)
        {
            IQueryable<Seat> query = _context.Seats
                .Where(s =>
                    s.EventId == eventId &&
                    s.SeatNumber == seatNumber);

            if (excludeSeatId.HasValue)
            {
                query = query.Where(
                    s => s.SeatId != excludeSeatId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveBookingAsync(int seatId)
        {
            return await _context.BookingSeats
                .AnyAsync(bs =>
                    bs.SeatId == seatId &&
                    bs.IsActive);
        }

        public async Task AddSeatAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
        }

        public async Task AddSeatsAsync(
            IEnumerable<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
        }

        public void UpdateSeat(Seat seat)
        {
            _context.Seats.Update(seat);
        }

        public void DeleteSeat(Seat seat)
        {
            _context.Seats.Remove(seat);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}