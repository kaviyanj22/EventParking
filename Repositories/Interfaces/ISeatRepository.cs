using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetSeatsByEventIdAsync(int eventId);

        Task<Seat?> GetSeatByIdAsync(int seatId);

        Task<bool> EventExistsAsync(int eventId);

        Task<int> GetEventCapacityAsync(int eventId);

        Task<bool> SeatNumberExistsAsync(
            int eventId,
            string seatNumber,
            int? excludeSeatId = null
        );

        Task<bool> HasActiveBookingAsync(int seatId);

        Task AddSeatAsync(Seat seat);

        Task AddSeatsAsync(IEnumerable<Seat> seats);

        void UpdateSeat(Seat seat);

        void DeleteSeat(Seat seat);

        Task<bool> SaveChangesAsync();
    }
}