using Event_parking.DTOs.Event;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly ICategoryRepository _categoryRepository;

        public EventService(
            IEventRepository eventRepository,
            IVenueRepository venueRepository,
            ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync(
            EventFilterDto? filter = null)
        {
            var events = await _eventRepository.GetAllAsync(filter);

            return events.Select(MapToResponseDto);
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
            {
                return null;
            }

            return MapToResponseDto(eventEntity);
        }

        public async Task<EventResponseDto> CreateAsync(
            EventCreateDto createDto)
        {
            ValidateTimeRange(
                createDto.StartTime,
                createDto.EndTime);

            var venue =
                await _venueRepository.GetByIdAsync(createDto.VenueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Selected venue was not found.");
            }

            var category =
                await _categoryRepository.GetByIdAsync(
                    createDto.CategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Selected category was not found.");
            }

            if (createDto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    $"Event capacity cannot exceed venue capacity of {venue.Capacity}.");
            }

            var hasOverlap =
                await _eventRepository.HasVenueOverlapAsync(
                    createDto.VenueId,
                    createDto.EventDate,
                    createDto.StartTime,
                    createDto.EndTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "The selected venue already has an event during this time period.");
            }

            var eventEntity = new Event
            {
                EventName = createDto.EventName.Trim(),
                VenueId = createDto.VenueId,
                CategoryId = createDto.CategoryId,
                EventDate = createDto.EventDate,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                TicketPrice = createDto.TicketPrice,
                Capacity = createDto.Capacity,
                ParkingFee = createDto.ParkingFee,
                CreatedAt = DateTime.UtcNow
            };

            var createdEvent =
                await _eventRepository.CreateAsync(eventEntity);

            var result =
                await _eventRepository.GetByIdAsync(
                    createdEvent.EventId);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Event was created but could not be retrieved.");
            }

            return MapToResponseDto(result);
        }

        public async Task<EventResponseDto?> UpdateAsync(
            int id,
            EventUpdateDto updateDto)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
            {
                return null;
            }

            ValidateTimeRange(
                updateDto.StartTime,
                updateDto.EndTime);

            var venue =
                await _venueRepository.GetByIdAsync(
                    updateDto.VenueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Selected venue was not found.");
            }

            var category =
                await _categoryRepository.GetByIdAsync(
                    updateDto.CategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Selected category was not found.");
            }

            if (updateDto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    $"Event capacity cannot exceed venue capacity of {venue.Capacity}.");
            }

            var bookedSeatCount =
                await _eventRepository.GetBookedSeatCountAsync(id);

            if (updateDto.Capacity < bookedSeatCount)
            {
                throw new InvalidOperationException(
                    $"Event capacity cannot be less than the {bookedSeatCount} seats already booked.");
            }

            var hasOverlap =
                await _eventRepository.HasVenueOverlapAsync(
                    updateDto.VenueId,
                    updateDto.EventDate,
                    updateDto.StartTime,
                    updateDto.EndTime,
                    id);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "The selected venue already has another event during this time period.");
            }

            eventEntity.EventName =
                updateDto.EventName.Trim();

            eventEntity.VenueId =
                updateDto.VenueId;

            eventEntity.CategoryId =
                updateDto.CategoryId;

            eventEntity.EventDate =
                updateDto.EventDate;

            eventEntity.StartTime =
                updateDto.StartTime;

            eventEntity.EndTime =
                updateDto.EndTime;

            eventEntity.TicketPrice =
                updateDto.TicketPrice;

            eventEntity.Capacity =
                updateDto.Capacity;

            eventEntity.ParkingFee =
                updateDto.ParkingFee;

            eventEntity.UpdatedAt =
                DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);

            var result =
                await _eventRepository.GetByIdAsync(id);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Event was updated but could not be retrieved.");
            }

            return MapToResponseDto(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
            {
                return false;
            }

            var hasActiveBookings =
                await _eventRepository.HasActiveBookingsAsync(id);

            if (hasActiveBookings)
            {
                throw new InvalidOperationException(
                    "This event cannot be deleted because it has active bookings.");
            }

            return await _eventRepository.DeleteAsync(eventEntity);
        }

        private static void ValidateTimeRange(
            TimeSpan startTime,
            TimeSpan endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException(
                    "Start time must be earlier than end time.");
            }
        }

        private static EventResponseDto MapToResponseDto(
            Event eventEntity)
        {
            return new EventResponseDto
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,

                VenueId = eventEntity.VenueId,
                VenueName =
                    eventEntity.Venue?.VenueName ?? string.Empty,

                CategoryId = eventEntity.CategoryId,
                CategoryName =
                    eventEntity.Category?.CategoryName ?? string.Empty,

                EventDate = eventEntity.EventDate,
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                TicketPrice = eventEntity.TicketPrice,
                Capacity = eventEntity.Capacity,
                ParkingFee = eventEntity.ParkingFee,
                CreatedAt = eventEntity.CreatedAt,
                UpdatedAt = eventEntity.UpdatedAt
            };
        }
    }
}