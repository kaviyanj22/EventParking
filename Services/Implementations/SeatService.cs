using Event_parking.DTOs.Seat;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly ISeatSectionRepository _seatSectionRepository;
        private readonly IEventRepository _eventRepository;

        public SeatService(
            ISeatRepository seatRepository,
            ISeatSectionRepository seatSectionRepository,
            IEventRepository eventRepository)
        {
            _seatRepository = seatRepository;
            _seatSectionRepository = seatSectionRepository;
            _eventRepository = eventRepository;
        }

        // ==========================================
        // GET ALL SEATS FOR AN EVENT
        // ==========================================
        public async Task<IEnumerable<SeatResponseDto>>
            GetSeatsByEventIdAsync(int eventId)
        {
            var seats =
                await _seatRepository
                    .GetSeatsByEventIdAsync(eventId);

            return seats.Select(MapToResponseDto);
        }

        // ==========================================
        // GET SINGLE SEAT
        // ==========================================
        public async Task<SeatResponseDto?>
            GetSeatByIdAsync(
                int eventId,
                int seatId)
        {
            var seat =
                await _seatRepository
                    .GetSeatByIdAsync(seatId);

            if (seat == null ||
                seat.EventId != eventId)
            {
                return null;
            }

            return MapToResponseDto(seat);
        }

        // ==========================================
        // CREATE FULL SEAT MAP
        // ==========================================
        public async Task<(bool Success, string Message)>
            CreateSeatMapAsync(
                int eventId,
                SeatMapCreateDto dto)
        {
            bool eventExists =
                await _seatRepository
                    .EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found."
                );
            }

            // ======================================
            // ACTIVE BOOKING SAFEGUARD
            // ======================================
            bool hasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(eventId);

            if (hasActiveBookings)
            {
                return (
                    false,
                    "Seat layout cannot be changed because this event has active bookings."
                );
            }

            if (dto.Seats == null ||
                dto.Seats.Count == 0)
            {
                return (
                    false,
                    "At least one seat is required."
                );
            }

            int eventCapacity =
                await _seatRepository
                    .GetEventCapacityAsync(eventId);

            if (dto.Seats.Count != eventCapacity)
            {
                return (
                    false,
                    $"Seat count must exactly match event capacity. " +
                    $"Event capacity is {eventCapacity}, " +
                    $"but {dto.Seats.Count} seats were provided."
                );
            }

            var existingSeats =
                await _seatRepository
                    .GetSeatsByEventIdAsync(eventId);

            if (existingSeats.Any())
            {
                return (
                    false,
                    "A seat map already exists for this event."
                );
            }

            var duplicateSeatNumbers =
                dto.Seats
                    .GroupBy(
                        seat => seat.SeatNumber.Trim(),
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();

            if (duplicateSeatNumbers.Any())
            {
                return (
                    false,
                    "Duplicate seat numbers are not allowed: " +
                    string.Join(
                        ", ",
                        duplicateSeatNumbers
                    )
                );
            }

            // ======================================
            // VALIDATE SEAT SECTIONS
            // ======================================
            Dictionary<int, SeatSection>
                sectionDictionary = new();

            List<int> sectionIds =
                dto.Seats
                    .Where(seat =>
                        seat.SeatSectionId.HasValue)
                    .Select(seat =>
                        seat.SeatSectionId!.Value)
                    .Distinct()
                    .ToList();

            foreach (int sectionId in sectionIds)
            {
                SeatSection? section =
                    await _seatSectionRepository
                        .GetByIdAsync(
                            eventId,
                            sectionId
                        );

                if (section == null)
                {
                    return (
                        false,
                        $"Seat section {sectionId} does not belong to this event."
                    );
                }

                sectionDictionary[sectionId] =
                    section;
            }

            List<Seat> seats =
                dto.Seats
                    .Select(dtoSeat =>
                    {
                        Seat seat = new Seat
                        {
                            EventId =
                                eventId,

                            SeatSectionId =
                                dtoSeat.SeatSectionId,

                            SeatNumber =
                                dtoSeat.SeatNumber.Trim(),

                            RowName =
                                string.IsNullOrWhiteSpace(
                                    dtoSeat.RowName)
                                    ? null
                                    : dtoSeat.RowName.Trim(),

                            ColumnNumber =
                                dtoSeat.ColumnNumber,

                            PositionX =
                                dtoSeat.PositionX,

                            PositionY =
                                dtoSeat.PositionY,

                            SeatType =
                                string.IsNullOrWhiteSpace(
                                    dtoSeat.SeatType)
                                    ? null
                                    : dtoSeat.SeatType.Trim(),

                            Price =
                                dtoSeat.Price,

                            Status =
                                "Available",

                            CreatedAt =
                                DateTime.UtcNow
                        };

                        if (dtoSeat.SeatSectionId.HasValue)
                        {
                            seat.SeatSection =
                                sectionDictionary[
                                    dtoSeat.SeatSectionId.Value
                                ];
                        }

                        return seat;
                    })
                    .ToList();

            await _seatRepository
                .AddSeatsAsync(seats);

            bool saved =
                await _seatRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Seat map could not be created."
                );
            }

            return (
                true,
                "Seat map created successfully."
            );
        }

        // ==========================================
        // CREATE SINGLE SEAT
        // ==========================================
        public async Task<(
            bool Success,
            string Message,
            SeatResponseDto? Data)>
            CreateSeatAsync(
                int eventId,
                SeatCreateDto dto)
        {
            bool eventExists =
                await _seatRepository
                    .EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found.",
                    null
                );
            }

            // ======================================
            // ACTIVE BOOKING SAFEGUARD
            // ======================================
            bool hasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(eventId);

            if (hasActiveBookings)
            {
                return (
                    false,
                    "Seat layout cannot be changed because this event has active bookings.",
                    null
                );
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SeatNumber))
            {
                return (
                    false,
                    "Seat number is required.",
                    null
                );
            }

            bool seatNumberExists =
                await _seatRepository
                    .SeatNumberExistsAsync(
                        eventId,
                        dto.SeatNumber.Trim()
                    );

            if (seatNumberExists)
            {
                return (
                    false,
                    "Seat number already exists for this event.",
                    null
                );
            }

            SeatSection? seatSection = null;

            if (dto.SeatSectionId.HasValue)
            {
                seatSection =
                    await _seatSectionRepository
                        .GetByIdAsync(
                            eventId,
                            dto.SeatSectionId.Value
                        );

                if (seatSection == null)
                {
                    return (
                        false,
                        "Selected seat section does not belong to this event.",
                        null
                    );
                }
            }

            int eventCapacity =
                await _seatRepository
                    .GetEventCapacityAsync(eventId);

            var existingSeats =
                await _seatRepository
                    .GetSeatsByEventIdAsync(eventId);

            if (existingSeats.Count() >=
                eventCapacity)
            {
                return (
                    false,
                    $"Cannot create another seat. " +
                    $"Event capacity is {eventCapacity}.",
                    null
                );
            }

            Seat seat = new Seat
            {
                EventId =
                    eventId,

                SeatSectionId =
                    dto.SeatSectionId,

                SeatSection =
                    seatSection,

                SeatNumber =
                    dto.SeatNumber.Trim(),

                RowName =
                    string.IsNullOrWhiteSpace(
                        dto.RowName)
                        ? null
                        : dto.RowName.Trim(),

                ColumnNumber =
                    dto.ColumnNumber,

                PositionX =
                    dto.PositionX,

                PositionY =
                    dto.PositionY,

                SeatType =
                    string.IsNullOrWhiteSpace(
                        dto.SeatType)
                        ? null
                        : dto.SeatType.Trim(),

                Price =
                    dto.Price,

                Status =
                    "Available",

                CreatedAt =
                    DateTime.UtcNow
            };

            await _seatRepository
                .AddSeatAsync(seat);

            bool saved =
                await _seatRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Seat could not be created.",
                    null
                );
            }

            return (
                true,
                "Seat created successfully.",
                MapToResponseDto(seat)
            );
        }

        // ==========================================
        // UPDATE SEAT
        // ==========================================
        public async Task<(bool Success, string Message)>
            UpdateSeatAsync(
                int eventId,
                int seatId,
                SeatUpdateDto dto)
        {
            Seat? seat =
                await _seatRepository
                    .GetSeatByIdAsync(seatId);

            if (seat == null ||
                seat.EventId != eventId)
            {
                return (
                    false,
                    "Seat not found."
                );
            }

            // ======================================
            // ACTIVE BOOKING SAFEGUARD
            // ======================================
            bool eventHasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(eventId);

            if (eventHasActiveBookings)
            {
                return (
                    false,
                    "Seat layout cannot be changed because this event has active bookings."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SeatNumber))
            {
                return (
                    false,
                    "Seat number is required."
                );
            }

            bool hasActiveBooking =
                await _seatRepository
                    .HasActiveBookingAsync(seatId);

            if (hasActiveBooking &&
                !string.Equals(
                    seat.SeatNumber,
                    dto.SeatNumber.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "A seat with an active booking cannot be renumbered."
                );
            }

            bool duplicateSeat =
                await _seatRepository
                    .SeatNumberExistsAsync(
                        eventId,
                        dto.SeatNumber.Trim(),
                        seatId
                    );

            if (duplicateSeat)
            {
                return (
                    false,
                    "Seat number already exists for this event."
                );
            }

            SeatSection? seatSection = null;

            if (dto.SeatSectionId.HasValue)
            {
                seatSection =
                    await _seatSectionRepository
                        .GetByIdAsync(
                            eventId,
                            dto.SeatSectionId.Value
                        );

                if (seatSection == null)
                {
                    return (
                        false,
                        "Selected seat section does not belong to this event."
                    );
                }
            }

            if (hasActiveBooking &&
                string.Equals(
                    dto.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "A seat with an active booking cannot be marked Available."
                );
            }

            if (!IsValidStatus(dto.Status))
            {
                return (
                    false,
                    "Invalid seat status. Allowed statuses are Available and Booked."
                );
            }

            seat.SeatSectionId =
                dto.SeatSectionId;

            seat.SeatSection =
                seatSection;

            seat.SeatNumber =
                dto.SeatNumber.Trim();

            seat.RowName =
                string.IsNullOrWhiteSpace(
                    dto.RowName)
                    ? null
                    : dto.RowName.Trim();

            seat.ColumnNumber =
                dto.ColumnNumber;

            seat.PositionX =
                dto.PositionX;

            seat.PositionY =
                dto.PositionY;

            seat.SeatType =
                string.IsNullOrWhiteSpace(
                    dto.SeatType)
                    ? null
                    : dto.SeatType.Trim();

            seat.Price =
                dto.Price;

            seat.Status =
                NormalizeStatus(
                    dto.Status
                );

            seat.UpdatedAt =
                DateTime.UtcNow;

            _seatRepository
                .UpdateSeat(seat);

            bool saved =
                await _seatRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Seat could not be updated."
                );
            }

            return (
                true,
                "Seat updated successfully."
            );
        }

        // ==========================================
        // DELETE SEAT
        // ==========================================
        public async Task<(bool Success, string Message)>
            DeleteSeatAsync(
                int eventId,
                int seatId)
        {
            Seat? seat =
                await _seatRepository
                    .GetSeatByIdAsync(seatId);

            if (seat == null ||
                seat.EventId != eventId)
            {
                return (
                    false,
                    "Seat not found."
                );
            }

            // ======================================
            // ACTIVE BOOKING SAFEGUARD
            // ======================================
            bool eventHasActiveBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(eventId);

            if (eventHasActiveBookings)
            {
                return (
                    false,
                    "Seat layout cannot be changed because this event has active bookings."
                );
            }

            bool hasActiveBooking =
                await _seatRepository
                    .HasActiveBookingAsync(seatId);

            if (hasActiveBooking)
            {
                return (
                    false,
                    "A seat with an active booking cannot be deleted."
                );
            }

            _seatRepository
                .DeleteSeat(seat);

            bool saved =
                await _seatRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Seat could not be deleted."
                );
            }

            return (
                true,
                "Seat deleted successfully."
            );
        }

        // ==========================================
        // MAPPING
        // ==========================================
        private static SeatResponseDto
            MapToResponseDto(
                Seat seat)
        {
            return new SeatResponseDto
            {
                SeatId =
                    seat.SeatId,

                EventId =
                    seat.EventId,

                SeatSectionId =
                    seat.SeatSectionId,

                SectionName =
                    seat.SeatSection?.SectionName,

                SeatNumber =
                    seat.SeatNumber,

                RowName =
                    seat.RowName,

                ColumnNumber =
                    seat.ColumnNumber,

                PositionX =
                    seat.PositionX,

                PositionY =
                    seat.PositionY,

                SeatType =
                    seat.SeatType,

                Price =
                    seat.Price,

                Status =
                    seat.Status
            };
        }

        // ==========================================
        // STATUS VALIDATION
        // ==========================================
        private static bool IsValidStatus(
            string status)
        {
            return string.Equals(
                       status,
                       "Available",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       status,
                       "Booked",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeStatus(
            string status)
        {
            if (string.Equals(
                    status,
                    "Booked",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Booked";
            }

            return "Available";
        }
    }
}