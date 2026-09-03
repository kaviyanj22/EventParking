using Event_parking.DTOs.Venue;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<IEnumerable<VenueResponseDto>> GetAllAsync()
        {
            var venues = await _venueRepository.GetAllAsync();

            return venues.Select(MapToResponseDto);
        }

        public async Task<VenueResponseDto?> GetByIdAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return null;
            }

            return MapToResponseDto(venue);
        }

        public async Task<VenueResponseDto> CreateAsync(
            VenueCreateDto createDto)
        {
            var venue = new Venue
            {
                VenueName = createDto.VenueName.Trim(),
                Address = createDto.Address.Trim(),
                Capacity = createDto.Capacity,
                CreatedAt = DateTime.UtcNow
            };

            var createdVenue =
                await _venueRepository.CreateAsync(venue);

            return MapToResponseDto(createdVenue);
        }

        public async Task<VenueResponseDto?> UpdateAsync(
            int id,
            VenueUpdateDto updateDto)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return null;
            }

            venue.VenueName = updateDto.VenueName.Trim();
            venue.Address = updateDto.Address.Trim();
            venue.Capacity = updateDto.Capacity;
            venue.UpdatedAt = DateTime.UtcNow;

            var updatedVenue =
                await _venueRepository.UpdateAsync(venue);

            return MapToResponseDto(updatedVenue);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return false;
            }

            var hasUpcomingEvents =
                await _venueRepository.HasUpcomingEventsAsync(id);

            if (hasUpcomingEvents)
            {
                throw new InvalidOperationException(
                    "This venue cannot be deleted because it has upcoming events.");
            }

            return await _venueRepository.DeleteAsync(venue);
        }

        public async Task<bool> IsAvailableAsync(
            int venueId,
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeEventId = null)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException(
                    "Start time must be earlier than end time.");
            }

            var venue = await _venueRepository.GetByIdAsync(venueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Venue not found.");
            }

            return await _venueRepository.IsAvailableAsync(
                venueId,
                eventDate,
                startTime,
                endTime,
                excludeEventId);
        }

        public async Task<IEnumerable<VenueResponseDto>>
            GetAvailableVenuesAsync(
                DateTime eventDate,
                TimeSpan startTime,
                TimeSpan endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException(
                    "Start time must be earlier than end time.");
            }

            var venues =
                await _venueRepository.GetAvailableVenuesAsync(
                    eventDate,
                    startTime,
                    endTime);

            return venues.Select(MapToResponseDto);
        }

        private static VenueResponseDto MapToResponseDto(
            Venue venue)
        {
            return new VenueResponseDto
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Address = venue.Address,
                Capacity = venue.Capacity,
                CreatedAt = venue.CreatedAt,
                UpdatedAt = venue.UpdatedAt
            };
        }
    }
}