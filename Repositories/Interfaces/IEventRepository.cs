using Event_parking.DTOs.Event;
using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync(EventFilterDto? filter = null);

        Task<Event?> GetByIdAsync(int id);

        Task<Event> CreateAsync(Event eventEntity);

        Task<Event> UpdateAsync(Event eventEntity);

        Task<bool> DeleteAsync(Event eventEntity);

        Task<bool> HasActiveBookingsAsync(int eventId);

        Task<int> GetBookedSeatCountAsync(int eventId);

        Task<bool> HasVenueOverlapAsync(
            int venueId,
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeEventId = null
        );
    }
}