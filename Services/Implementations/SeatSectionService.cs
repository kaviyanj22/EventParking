using Event_parking.DTOs.SeatSection;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class SeatSectionService : ISeatSectionService
    {
        private readonly ISeatSectionRepository _seatSectionRepository;
        private readonly IEventRepository _eventRepository;

        public SeatSectionService(
            ISeatSectionRepository seatSectionRepository,
            IEventRepository eventRepository)
        {
            _seatSectionRepository =
                seatSectionRepository;

            _eventRepository =
                eventRepository;
        }

        // ==========================================
        // GET ALL SECTIONS FOR EVENT
        // ==========================================
        public async Task<
            ServiceResult<List<SeatSectionResponseDto>>>
            GetByEventIdAsync(int eventId)
        {
            var eventEntity =
                await _eventRepository
                    .GetByIdAsync(eventId);

            if (eventEntity == null)
            {
                return ServiceResult<
                    List<SeatSectionResponseDto>>
                    .Fail(
                        "Event not found."
                    );
            }

            List<SeatSection> sections =
                await _seatSectionRepository
                    .GetByEventIdAsync(eventId);

            List<SeatSectionResponseDto> response =
                sections
                    .Select(MapToResponse)
                    .ToList();

            return ServiceResult<
                List<SeatSectionResponseDto>>
                .Ok(
                    response,
                    "Seat sections retrieved successfully."
                );
        }

        // ==========================================
        // GET SINGLE SECTION
        // ==========================================
        public async Task<
            ServiceResult<SeatSectionResponseDto>>
            GetByIdAsync(
                int eventId,
                int seatSectionId)
        {
            SeatSection? section =
                await _seatSectionRepository
                    .GetByIdAsync(
                        eventId,
                        seatSectionId
                    );

            if (section == null)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "Seat section not found."
                    );
            }

            return ServiceResult<
                SeatSectionResponseDto>
                .Ok(
                    MapToResponse(section),
                    "Seat section retrieved successfully."
                );
        }

        // ==========================================
        // CREATE SECTION
        // ==========================================
        public async Task<
            ServiceResult<SeatSectionResponseDto>>
            CreateAsync(
                int eventId,
                SeatSectionCreateDto dto)
        {
            var eventEntity =
                await _eventRepository
                    .GetByIdAsync(eventId);

            if (eventEntity == null)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "Event not found."
                    );
            }

            // ======================================
            // SAFEGUARD
            // ======================================
            bool hasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(
                        eventId
                    );

            if (hasActiveBookings)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "Seat layout cannot be changed because this event has active bookings."
                    );
            }

            string sectionName =
                dto.SectionName.Trim();

            bool duplicate =
                await _seatSectionRepository
                    .ExistsByNameAsync(
                        eventId,
                        sectionName
                    );

            if (duplicate)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "A seat section with this name already exists."
                    );
            }

            SeatSection section =
                new SeatSection
                {
                    EventId =
                        eventId,

                    SectionName =
                        sectionName,

                    Description =
                        string.IsNullOrWhiteSpace(
                            dto.Description)
                            ? null
                            : dto.Description.Trim(),

                    LayoutImageUrl =
                        string.IsNullOrWhiteSpace(
                            dto.LayoutImageUrl)
                            ? null
                            : dto.LayoutImageUrl.Trim(),

                    DisplayOrder =
                        dto.DisplayOrder,

                    CreatedAt =
                        DateTime.UtcNow
                };

            await _seatSectionRepository
                .AddAsync(section);

            await _seatSectionRepository
                .SaveChangesAsync();

            return ServiceResult<
                SeatSectionResponseDto>
                .Ok(
                    MapToResponse(section),
                    "Seat section created successfully."
                );
        }

        // ==========================================
        // UPDATE SECTION
        // ==========================================
        public async Task<
            ServiceResult<SeatSectionResponseDto>>
            UpdateAsync(
                int eventId,
                int seatSectionId,
                SeatSectionUpdateDto dto)
        {
            SeatSection? section =
                await _seatSectionRepository
                    .GetByIdAsync(
                        eventId,
                        seatSectionId
                    );

            if (section == null)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "Seat section not found."
                    );
            }

            // ======================================
            // SAFEGUARD
            // ======================================
            bool hasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(
                        eventId
                    );

            if (hasActiveBookings)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "Seat layout cannot be changed because this event has active bookings."
                    );
            }

            string sectionName =
                dto.SectionName.Trim();

            bool duplicate =
                await _seatSectionRepository
                    .ExistsByNameAsync(
                        eventId,
                        sectionName,
                        seatSectionId
                    );

            if (duplicate)
            {
                return ServiceResult<
                    SeatSectionResponseDto>
                    .Fail(
                        "A seat section with this name already exists."
                    );
            }

            section.SectionName =
                sectionName;

            section.Description =
                string.IsNullOrWhiteSpace(
                    dto.Description)
                    ? null
                    : dto.Description.Trim();

            section.LayoutImageUrl =
                string.IsNullOrWhiteSpace(
                    dto.LayoutImageUrl)
                    ? null
                    : dto.LayoutImageUrl.Trim();

            section.DisplayOrder =
                dto.DisplayOrder;

            section.UpdatedAt =
                DateTime.UtcNow;

            _seatSectionRepository
                .Update(section);

            await _seatSectionRepository
                .SaveChangesAsync();

            return ServiceResult<
                SeatSectionResponseDto>
                .Ok(
                    MapToResponse(section),
                    "Seat section updated successfully."
                );
        }

        // ==========================================
        // DELETE SECTION
        // ==========================================
        public async Task<
            ServiceResult<bool>>
            DeleteAsync(
                int eventId,
                int seatSectionId)
        {
            SeatSection? section =
                await _seatSectionRepository
                    .GetByIdAsync(
                        eventId,
                        seatSectionId
                    );

            if (section == null)
            {
                return ServiceResult<bool>
                    .Fail(
                        "Seat section not found."
                    );
            }

            // ======================================
            // SAFEGUARD
            // ======================================
            bool hasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(
                        eventId
                    );

            if (hasActiveBookings)
            {
                return ServiceResult<bool>
                    .Fail(
                        "Seat layout cannot be changed because this event has active bookings."
                    );
            }

            if (section.Seats.Any())
            {
                return ServiceResult<bool>
                    .Fail(
                        "This seat section contains seats. Delete or move the seats before deleting the section."
                    );
            }

            _seatSectionRepository
                .Delete(section);

            await _seatSectionRepository
                .SaveChangesAsync();

            return ServiceResult<bool>
                .Ok(
                    true,
                    "Seat section deleted successfully."
                );
        }

        // ==========================================
        // MAPPING
        // ==========================================
        private static SeatSectionResponseDto
            MapToResponse(
                SeatSection section)
        {
            return new SeatSectionResponseDto
            {
                SeatSectionId =
                    section.SeatSectionId,

                EventId =
                    section.EventId,

                SectionName =
                    section.SectionName,

                Description =
                    section.Description,

                LayoutImageUrl =
                    section.LayoutImageUrl,

                DisplayOrder =
                    section.DisplayOrder,

                SeatCount =
                    section.Seats?.Count ?? 0
            };
        }
    }
}