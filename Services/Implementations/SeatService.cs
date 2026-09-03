using Event_parking.DTOs.Seat;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        // ==========================================
        // GET ALL SEATS FOR AN EVENT
        // ==========================================
        public async Task<IEnumerable<SeatResponseDto>>
            GetSeatsByEventIdAsync(int eventId)
        {
            var seats =
                await _seatRepository.GetSeatsByEventIdAsync(eventId);

            return seats.Select(MapToResponseDto);
        }

        // ==========================================
        // GET SINGLE SEAT
        // ==========================================
        public async Task<SeatResponseDto?>
            GetSeatByIdAsync(int eventId, int seatId)
        {
            var seat =
                await _seatRepository.GetSeatByIdAsync(seatId);

            if (seat == null || seat.EventId != eventId)
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
            // Check event
            var eventExists =
                await _seatRepository.EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found."
                );
            }

            if (dto.Seats == null || dto.Seats.Count == 0)
            {
                return (
                    false,
                    "At least one seat is required."
                );
            }

            // Get event capacity
            var eventCapacity =
                await _seatRepository.GetEventCapacityAsync(eventId);

            // Seat count must equal event capacity
            if (dto.Seats.Count != eventCapacity)
            {
                return (
                    false,
                    $"Seat count must exactly match event capacity. " +
                    $"Event capacity is {eventCapacity}, " +
                    $"but {dto.Seats.Count} seats were provided."
                );
            }

            // Do not generate another map if seats already exist
            var existingSeats =
                await _seatRepository.GetSeatsByEventIdAsync(eventId);

            if (existingSeats.Any())
            {
                return (
                    false,
                    "A seat map already exists for this event."
                );
            }

            // Check duplicate seat numbers inside request
            var duplicateSeatNumbers = dto.Seats
                .GroupBy(
                    s => s.SeatNumber.Trim(),
                    StringComparer.OrdinalIgnoreCase
                )
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateSeatNumbers.Any())
            {
                return (
                    false,
                    "Duplicate seat numbers are not allowed: " +
                    string.Join(", ", duplicateSeatNumbers)
                );
            }

            var seats = dto.Seats.Select(dtoSeat =>
                new Seat
                {
                    EventId = eventId,
                    SeatNumber = dtoSeat.SeatNumber.Trim(),
                    RowName = dtoSeat.RowName?.Trim(),
                    ColumnNumber = dtoSeat.ColumnNumber,
                    SeatType = dtoSeat.SeatType?.Trim(),
                    Price = dtoSeat.Price,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow
                }
            ).ToList();

            await _seatRepository.AddSeatsAsync(seats);

            var saved =
                await _seatRepository.SaveChangesAsync();

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
            var eventExists =
                await _seatRepository.EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found.",
                    null
                );
            }

            if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            {
                return (
                    false,
                    "Seat number is required.",
                    null
                );
            }

            var seatNumberExists =
                await _seatRepository.SeatNumberExistsAsync(
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

            // Prevent seat count going above event capacity
            var eventCapacity =
                await _seatRepository.GetEventCapacityAsync(eventId);

            var existingSeats =
                await _seatRepository.GetSeatsByEventIdAsync(eventId);

            if (existingSeats.Count() >= eventCapacity)
            {
                return (
                    false,
                    $"Cannot create another seat. " +
                    $"Event capacity is {eventCapacity}.",
                    null
                );
            }

            var seat = new Seat
            {
                EventId = eventId,
                SeatNumber = dto.SeatNumber.Trim(),
                RowName = dto.RowName?.Trim(),
                ColumnNumber = dto.ColumnNumber,
                SeatType = dto.SeatType?.Trim(),
                Price = dto.Price,
                Status = "Available",
                CreatedAt = DateTime.UtcNow
            };

            await _seatRepository.AddSeatAsync(seat);

            var saved =
                await _seatRepository.SaveChangesAsync();

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
            var seat =
                await _seatRepository.GetSeatByIdAsync(seatId);

            if (seat == null || seat.EventId != eventId)
            {
                return (
                    false,
                    "Seat not found."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.SeatNumber))
            {
                return (
                    false,
                    "Seat number is required."
                );
            }

            var hasActiveBooking =
                await _seatRepository.HasActiveBookingAsync(seatId);

            // BRD: booked seat cannot be renumbered
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

            var duplicateSeat =
                await _seatRepository.SeatNumberExistsAsync(
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

            // Do not allow booked seat to be manually made Available
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

            seat.SeatNumber = dto.SeatNumber.Trim();
            seat.RowName = dto.RowName?.Trim();
            seat.ColumnNumber = dto.ColumnNumber;
            seat.SeatType = dto.SeatType?.Trim();
            seat.Price = dto.Price;
            seat.Status = NormalizeStatus(dto.Status);
            seat.UpdatedAt = DateTime.UtcNow;

            _seatRepository.UpdateSeat(seat);

            var saved =
                await _seatRepository.SaveChangesAsync();

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
            var seat =
                await _seatRepository.GetSeatByIdAsync(seatId);

            if (seat == null || seat.EventId != eventId)
            {
                return (
                    false,
                    "Seat not found."
                );
            }

            var hasActiveBooking =
                await _seatRepository.HasActiveBookingAsync(seatId);

            if (hasActiveBooking)
            {
                return (
                    false,
                    "A seat with an active booking cannot be deleted."
                );
            }

            _seatRepository.DeleteSeat(seat);

            var saved =
                await _seatRepository.SaveChangesAsync();

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
        private static SeatResponseDto MapToResponseDto(
            Seat seat)
        {
            return new SeatResponseDto
            {
                SeatId = seat.SeatId,
                EventId = seat.EventId,
                SeatNumber = seat.SeatNumber,
                RowName = seat.RowName,
                ColumnNumber = seat.ColumnNumber,
                SeatType = seat.SeatType,
                Price = seat.Price,
                Status = seat.Status
            };
        }

        // ==========================================
        // STATUS VALIDATION
        // ==========================================
        private static bool IsValidStatus(string status)
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

        private static string NormalizeStatus(string status)
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