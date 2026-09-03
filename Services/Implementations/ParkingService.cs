using Event_parking.DTOs.Parking;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _parkingRepository;

        public ParkingService(IParkingRepository parkingRepository)
        {
            _parkingRepository = parkingRepository;
        }

        // ==========================================
        // GET ALL PARKING SLOTS FOR EVENT
        // ==========================================
        public async Task<IEnumerable<ParkingSlotResponseDto>>
            GetParkingSlotsByEventIdAsync(int eventId)
        {
            var parkingSlots =
                await _parkingRepository
                    .GetParkingSlotsByEventIdAsync(eventId);

            return parkingSlots.Select(MapToResponseDto);
        }

        // ==========================================
        // GET SINGLE PARKING SLOT
        // ==========================================
        public async Task<ParkingSlotResponseDto?>
            GetParkingSlotByIdAsync(
                int eventId,
                int parkingSlotId)
        {
            var parkingSlot =
                await _parkingRepository
                    .GetParkingSlotByIdAsync(parkingSlotId);

            if (parkingSlot == null ||
                parkingSlot.EventId != eventId)
            {
                return null;
            }

            return MapToResponseDto(parkingSlot);
        }

        // ==========================================
        // CREATE FULL PARKING LAYOUT
        // ==========================================
        public async Task<(bool Success, string Message)>
            CreateParkingLayoutAsync(
                int eventId,
                ParkingLayoutCreateDto dto)
        {
            var eventExists =
                await _parkingRepository
                    .EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found."
                );
            }

            if (dto.ParkingSlots == null ||
                dto.ParkingSlots.Count == 0)
            {
                return (
                    false,
                    "At least one parking slot is required."
                );
            }

            // Check whether layout already exists
            var existingSlots =
                await _parkingRepository
                    .GetParkingSlotsByEventIdAsync(eventId);

            if (existingSlots.Any())
            {
                return (
                    false,
                    "A parking layout already exists for this event."
                );
            }

            // Check duplicate slot numbers inside request
            var duplicateSlotNumbers = dto.ParkingSlots
                .GroupBy(
                    p => p.SlotNumber.Trim(),
                    StringComparer.OrdinalIgnoreCase
                )
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateSlotNumbers.Any())
            {
                return (
                    false,
                    "Duplicate parking slot numbers are not allowed: " +
                    string.Join(", ", duplicateSlotNumbers)
                );
            }

            var parkingSlots = dto.ParkingSlots
                .Select(dtoSlot =>
                    new ParkingSlot
                    {
                        EventId = eventId,
                        SlotNumber =
                            dtoSlot.SlotNumber.Trim(),

                        Zone =
                            dtoSlot.Zone?.Trim(),

                        Fee =
                            dtoSlot.Fee,

                        Status =
                            "Available",

                        CreatedAt =
                            DateTime.UtcNow
                    }
                )
                .ToList();

            await _parkingRepository
                .AddParkingSlotsAsync(parkingSlots);

            var saved =
                await _parkingRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Parking layout could not be created."
                );
            }

            return (
                true,
                "Parking layout created successfully."
            );
        }

        // ==========================================
        // CREATE SINGLE PARKING SLOT
        // ==========================================
        public async Task<(
            bool Success,
            string Message,
            ParkingSlotResponseDto? Data)>
            CreateParkingSlotAsync(
                int eventId,
                ParkingSlotCreateDto dto)
        {
            var eventExists =
                await _parkingRepository
                    .EventExistsAsync(eventId);

            if (!eventExists)
            {
                return (
                    false,
                    "Event not found.",
                    null
                );
            }

            if (string.IsNullOrWhiteSpace(dto.SlotNumber))
            {
                return (
                    false,
                    "Parking slot number is required.",
                    null
                );
            }

            var slotExists =
                await _parkingRepository
                    .SlotNumberExistsAsync(
                        eventId,
                        dto.SlotNumber.Trim()
                    );

            if (slotExists)
            {
                return (
                    false,
                    "Parking slot number already exists for this event.",
                    null
                );
            }

            var parkingSlot = new ParkingSlot
            {
                EventId = eventId,

                SlotNumber =
                    dto.SlotNumber.Trim(),

                Zone =
                    dto.Zone?.Trim(),

                Fee =
                    dto.Fee,

                Status =
                    "Available",

                CreatedAt =
                    DateTime.UtcNow
            };

            await _parkingRepository
                .AddParkingSlotAsync(parkingSlot);

            var saved =
                await _parkingRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Parking slot could not be created.",
                    null
                );
            }

            return (
                true,
                "Parking slot created successfully.",
                MapToResponseDto(parkingSlot)
            );
        }

        // ==========================================
        // UPDATE PARKING SLOT
        // ==========================================
        public async Task<(bool Success, string Message)>
            UpdateParkingSlotAsync(
                int eventId,
                int parkingSlotId,
                ParkingSlotUpdateDto dto)
        {
            var parkingSlot =
                await _parkingRepository
                    .GetParkingSlotByIdAsync(parkingSlotId);

            if (parkingSlot == null ||
                parkingSlot.EventId != eventId)
            {
                return (
                    false,
                    "Parking slot not found."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.SlotNumber))
            {
                return (
                    false,
                    "Parking slot number is required."
                );
            }

            var hasActiveReservation =
                await _parkingRepository
                    .HasActiveReservationAsync(parkingSlotId);

            // Reserved slot cannot be renumbered
            if (hasActiveReservation &&
                !string.Equals(
                    parkingSlot.SlotNumber,
                    dto.SlotNumber.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "A parking slot with an active reservation cannot be renumbered."
                );
            }

            var duplicateSlot =
                await _parkingRepository
                    .SlotNumberExistsAsync(
                        eventId,
                        dto.SlotNumber.Trim(),
                        parkingSlotId
                    );

            if (duplicateSlot)
            {
                return (
                    false,
                    "Parking slot number already exists for this event."
                );
            }

            if (!IsValidStatus(dto.Status))
            {
                return (
                    false,
                    "Invalid parking status. Allowed statuses are Available, Reserved and Occupied."
                );
            }

            // Active reservation cannot be made available manually
            if (hasActiveReservation &&
                string.Equals(
                    dto.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
            {
                return (
                    false,
                    "A parking slot with an active reservation cannot be marked Available."
                );
            }

            parkingSlot.SlotNumber =
                dto.SlotNumber.Trim();

            parkingSlot.Zone =
                dto.Zone?.Trim();

            parkingSlot.Fee =
                dto.Fee;

            parkingSlot.Status =
                NormalizeStatus(dto.Status);

            parkingSlot.UpdatedAt =
                DateTime.UtcNow;

            _parkingRepository
                .UpdateParkingSlot(parkingSlot);

            var saved =
                await _parkingRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Parking slot could not be updated."
                );
            }

            return (
                true,
                "Parking slot updated successfully."
            );
        }

        // ==========================================
        // DELETE PARKING SLOT
        // ==========================================
        public async Task<(bool Success, string Message)>
            DeleteParkingSlotAsync(
                int eventId,
                int parkingSlotId)
        {
            var parkingSlot =
                await _parkingRepository
                    .GetParkingSlotByIdAsync(parkingSlotId);

            if (parkingSlot == null ||
                parkingSlot.EventId != eventId)
            {
                return (
                    false,
                    "Parking slot not found."
                );
            }

            var hasActiveReservation =
                await _parkingRepository
                    .HasActiveReservationAsync(parkingSlotId);

            if (hasActiveReservation)
            {
                return (
                    false,
                    "A parking slot with an active reservation cannot be deleted."
                );
            }

            _parkingRepository
                .DeleteParkingSlot(parkingSlot);

            var saved =
                await _parkingRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return (
                    false,
                    "Parking slot could not be deleted."
                );
            }

            return (
                true,
                "Parking slot deleted successfully."
            );
        }

        // ==========================================
        // MAPPING
        // ==========================================
        private static ParkingSlotResponseDto
            MapToResponseDto(ParkingSlot parkingSlot)
        {
            return new ParkingSlotResponseDto
            {
                ParkingSlotId =
                    parkingSlot.ParkingSlotId,

                EventId =
                    parkingSlot.EventId,

                SlotNumber =
                    parkingSlot.SlotNumber,

                Zone =
                    parkingSlot.Zone,

                Fee =
                    parkingSlot.Fee,

                Status =
                    parkingSlot.Status
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
                       "Reserved",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       status,
                       "Occupied",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeStatus(string status)
        {
            if (string.Equals(
                    status,
                    "Reserved",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Reserved";
            }

            if (string.Equals(
                    status,
                    "Occupied",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Occupied";
            }

            return "Available";
        }
    }
}