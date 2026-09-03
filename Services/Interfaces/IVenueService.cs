using Event_parking.DTOs.Venue;

namespace Event_parking.Services.Interfaces
{
    public interface IVenueService
    {
        Task<IEnumerable<VenueResponseDto>> GetAllAsync();

        Task<VenueResponseDto?> GetByIdAsync(int id);

        Task<VenueResponseDto> CreateAsync(
            VenueCreateDto createDto);

        Task<VenueResponseDto?> UpdateAsync(
            int id,
            VenueUpdateDto updateDto);

        Task<bool> DeleteAsync(int id);

        Task<bool> IsAvailableAsync(
            int venueId,
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeEventId = null);

        Task<IEnumerable<VenueResponseDto>> GetAvailableVenuesAsync(
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime);
    }
}