using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface IVenueRepository
    {
        Task<IEnumerable<Venue>> GetAllAsync();

        Task<Venue?> GetByIdAsync(int id);

        Task<Venue> CreateAsync(Venue venue);

        Task<Venue> UpdateAsync(Venue venue);

        Task<bool> DeleteAsync(Venue venue);

        Task<bool> HasUpcomingEventsAsync(int venueId);

        Task<bool> IsAvailableAsync(
            int venueId,
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeEventId = null
        );

        Task<IEnumerable<Venue>> GetAvailableVenuesAsync(
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime
        );
    }
}
