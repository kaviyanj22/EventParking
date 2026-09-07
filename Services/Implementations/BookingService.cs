using Event_parking.Configurations;
using Event_parking.DTOs.Booking;
using Event_parking.DTOs.Notification;
using Event_parking.Helpers;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Event_parking.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingNumberGenerator _bookingNumberGenerator;
        private readonly BookingSettings _bookingSettings;
        private readonly INotificationService _notificationService;

        public BookingService(
            IBookingRepository bookingRepository,
            BookingNumberGenerator bookingNumberGenerator,
            IOptions<BookingSettings> bookingSettings,
            INotificationService notificationService)
        {
            _bookingRepository = bookingRepository;
            _bookingNumberGenerator = bookingNumberGenerator;
            _bookingSettings = bookingSettings.Value;
            _notificationService = notificationService;
        }

        // ======================================
        // CREATE BOOKING
        // ======================================

        public async Task<ServiceResult<BookingResponseDto>>
            CreateBookingAsync(
                int customerId,
                BookingCreateDto dto)
        {
            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "At least one seat must be selected.");
            }

            if (dto.SeatIds.Any(id => id <= 0))
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "One or more selected seat IDs are invalid.");
            }

            if (dto.SeatIds.Count !=
                dto.SeatIds.Distinct().Count())
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "The same seat cannot be selected more than once.");
            }

            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                Customer? customer =
                    await _bookingRepository
                        .GetCustomerByIdAsync(
                            customerId);

                if (customer == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Customer was not found.");
                }

                if (!string.Equals(
                    customer.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Customer account is not active.");
                }

                if (!customer.EmailVerified)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Please verify your email before creating a booking.");
                }

                Event? eventItem =
                    await _bookingRepository
                        .GetEventByIdAsync(
                            dto.EventId);

                if (eventItem == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Event was not found.");
                }

                List<int> requestedSeatIds =
                    dto.SeatIds
                        .Distinct()
                        .ToList();

                List<Seat> seats =
                    await _bookingRepository
                        .GetSeatsByIdsAsync(
                            dto.EventId,
                            requestedSeatIds);

                if (seats.Count !=
                    requestedSeatIds.Count)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "One or more selected seats do not exist for this event.");
                }

                foreach (Seat seat in seats)
                {
                    if (!string.Equals(
                        seat.Status,
                        "Available",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Seat {seat.SeatNumber} is not available.");
                    }

                    bool hasActiveBooking =
                        await _bookingRepository
                            .HasActiveBookingSeatAsync(
                                seat.SeatId);

                    if (hasActiveBooking)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Seat {seat.SeatNumber} has already been booked.");
                    }
                }

                ParkingSlot? parkingSlot = null;

                if (dto.ParkingSlotId.HasValue)
                {
                    if (dto.ParkingSlotId.Value <= 0)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                "A valid parking slot is required.");
                    }

                    parkingSlot =
                        await _bookingRepository
                            .GetParkingSlotByIdAsync(
                                dto.EventId,
                                dto.ParkingSlotId.Value);

                    if (parkingSlot == null)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                "Parking slot was not found for this event.");
                    }

                    if (!string.Equals(
                        parkingSlot.Status,
                        "Available",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Parking slot {parkingSlot.SlotNumber} is not available.");
                    }

                    bool hasActiveParking =
                        await _bookingRepository
                            .HasActiveParkingReservationAsync(
                                parkingSlot.ParkingSlotId);

                    if (hasActiveParking)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Parking slot {parkingSlot.SlotNumber} has already been reserved.");
                    }
                }

                string bookingNumber;

                do
                {
                    bookingNumber =
                        _bookingNumberGenerator
                            .Generate();
                }
                while (await _bookingRepository
                    .BookingNumberExistsAsync(
                        bookingNumber));

                int holdMinutes =
                    _bookingSettings.HoldMinutes > 0
                        ? _bookingSettings.HoldMinutes
                        : 15;

                DateTime utcNow =
                    DateTime.UtcNow;

                Booking booking =
                    new Booking
                    {
                        BookingNumber =
                            bookingNumber,

                        CustomerId =
                            customerId,

                        EventId =
                            eventItem.EventId,

                        Status =
                            "Pending",

                        HoldExpiresAt =
                            utcNow.AddMinutes(
                                holdMinutes),

                        CreatedAt =
                            utcNow
                    };

                foreach (Seat seat in seats)
                {
                    decimal bookingPrice =
                        seat.Price
                        ?? eventItem.TicketPrice;

                    BookingSeat bookingSeat =
                        new BookingSeat
                        {
                            SeatId =
                                seat.SeatId,

                            PriceAtBooking =
                                bookingPrice,

                            IsActive =
                                true,

                            ReservedAt =
                                utcNow,

                            Seat =
                                seat
                        };

                    booking.BookingSeats
                        .Add(bookingSeat);

                    seat.Status =
                        "Booked";

                    seat.UpdatedAt =
                        utcNow;
                }

                if (parkingSlot != null)
                {
                    booking.ParkingReservation =
                        new ParkingReservation
                        {
                            ParkingSlotId =
                                parkingSlot.ParkingSlotId,

                            FeeAtReservation =
                                parkingSlot.Fee,

                            ReservedAt =
                                utcNow,

                            IsActive =
                                true,

                            ParkingSlot =
                                parkingSlot
                        };

                    parkingSlot.Status =
                        "Reserved";

                    parkingSlot.UpdatedAt =
                        utcNow;
                }

                await _bookingRepository
                    .AddBookingAsync(
                        booking);

                bool saved =
                    await _bookingRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction
                        .RollbackAsync();

                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Failed to create booking.");
                }

                await transaction
                    .CommitAsync();

                Booking? createdBooking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            booking.BookingId);

                if (createdBooking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was created but could not be retrieved.");
                }

                return ServiceResult<BookingResponseDto>
                    .Ok(
                        MapToBookingResponseDto(
                            createdBooking),
                        "Booking created successfully.");
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "An error occurred while creating the booking.");
            }
        }

        // ======================================
        // ADD SEATS TO EXISTING BOOKING
        // ======================================

        public async Task<ServiceResult<BookingResponseDto>>
            AddSeatsAsync(
                int bookingId,
                int customerId,
                BookingAddSeatsDto dto)
        {
            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "At least one seat must be selected.");
            }

            if (dto.SeatIds.Any(id => id <= 0))
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "One or more selected seat IDs are invalid.");
            }

            if (dto.SeatIds.Count !=
                dto.SeatIds.Distinct().Count())
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "The same seat cannot be selected more than once.");
            }

            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                Booking? booking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (booking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was not found.");
                }

                if (booking.CustomerId !=
                    customerId)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "You are not authorized to modify this booking.");
                }

                if (!string.Equals(
                    booking.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Seats can only be added to a pending booking.");
                }

                DateTime utcNow =
                    DateTime.UtcNow;

                if (booking.HoldExpiresAt.HasValue &&
                    booking.HoldExpiresAt.Value <= utcNow)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking hold has expired.");
                }

                Event? eventItem =
                    await _bookingRepository
                        .GetEventByIdAsync(
                            booking.EventId);

                if (eventItem == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Event was not found.");
                }

                List<int> requestedSeatIds =
                    dto.SeatIds
                        .Distinct()
                        .ToList();

                List<Seat> seats =
                    await _bookingRepository
                        .GetSeatsByIdsAsync(
                            booking.EventId,
                            requestedSeatIds);

                if (seats.Count !=
                    requestedSeatIds.Count)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "One or more selected seats do not exist for this event.");
                }

                foreach (Seat seat in seats)
                {
                    bool alreadyInBooking =
                        booking.BookingSeats
                            .Any(bookingSeat =>
                                bookingSeat.SeatId ==
                                    seat.SeatId
                                &&
                                bookingSeat.IsActive);

                    if (alreadyInBooking)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Seat {seat.SeatNumber} is already part of this booking.");
                    }

                    if (!string.Equals(
                        seat.Status,
                        "Available",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Seat {seat.SeatNumber} is not available.");
                    }

                    bool hasActiveBooking =
                        await _bookingRepository
                            .HasActiveBookingSeatAsync(
                                seat.SeatId);

                    if (hasActiveBooking)
                    {
                        return ServiceResult<BookingResponseDto>
                            .Fail(
                                $"Seat {seat.SeatNumber} has already been booked.");
                    }
                }

                foreach (Seat seat in seats)
                {
                    decimal bookingPrice =
                        seat.Price
                        ?? eventItem.TicketPrice;

                    BookingSeat bookingSeat =
                        new BookingSeat
                        {
                            BookingId =
                                booking.BookingId,

                            SeatId =
                                seat.SeatId,

                            PriceAtBooking =
                                bookingPrice,

                            IsActive =
                                true,

                            ReservedAt =
                                utcNow,

                            Seat =
                                seat
                        };

                    booking.BookingSeats
                        .Add(bookingSeat);

                    seat.Status =
                        "Booked";

                    seat.UpdatedAt =
                        utcNow;
                }

                booking.UpdatedAt =
                    utcNow;

                bool saved =
                    await _bookingRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction
                        .RollbackAsync();

                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Failed to add seats to booking.");
                }

                await transaction
                    .CommitAsync();

                Booking? updatedBooking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (updatedBooking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was updated but could not be retrieved.");
                }

                return ServiceResult<BookingResponseDto>
                    .Ok(
                        MapToBookingResponseDto(
                            updatedBooking),
                        "Seats added successfully.");
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "An error occurred while adding seats.");
            }
        }

        // ======================================
        // ADD PARKING
        // ======================================

        public async Task<ServiceResult<BookingResponseDto>>
            AddParkingAsync(
                int bookingId,
                int customerId,
                BookingParkingRequestDto dto)
        {
            if (dto.ParkingSlotId <= 0)
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "A valid parking slot is required.");
            }

            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                Booking? booking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (booking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was not found.");
                }

                if (booking.CustomerId !=
                    customerId)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "You are not authorized to modify this booking.");
                }

                if (!string.Equals(
                    booking.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Parking can only be added to a pending booking.");
                }

                DateTime utcNow =
                    DateTime.UtcNow;

                if (booking.HoldExpiresAt.HasValue &&
                    booking.HoldExpiresAt.Value <= utcNow)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking hold has expired.");
                }

                if (booking.ParkingReservation != null &&
                    booking.ParkingReservation.IsActive)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking already has an active parking reservation.");
                }

                ParkingSlot? parkingSlot =
                    await _bookingRepository
                        .GetParkingSlotByIdAsync(
                            booking.EventId,
                            dto.ParkingSlotId);

                if (parkingSlot == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Parking slot was not found for this event.");
                }

                if (!string.Equals(
                    parkingSlot.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            $"Parking slot {parkingSlot.SlotNumber} is not available.");
                }

                bool hasActiveParking =
                    await _bookingRepository
                        .HasActiveParkingReservationAsync(
                            parkingSlot.ParkingSlotId);

                if (hasActiveParking)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            $"Parking slot {parkingSlot.SlotNumber} has already been reserved.");
                }

                if (booking.ParkingReservation == null)
                {
                    booking.ParkingReservation =
                        new ParkingReservation
                        {
                            BookingId =
                                booking.BookingId,

                            ParkingSlotId =
                                parkingSlot.ParkingSlotId,

                            FeeAtReservation =
                                parkingSlot.Fee,

                            ReservedAt =
                                utcNow,

                            ReleasedAt =
                                null,

                            IsActive =
                                true,

                            ParkingSlot =
                                parkingSlot
                        };
                }
                else
                {
                    booking.ParkingReservation
                        .ParkingSlotId =
                            parkingSlot.ParkingSlotId;

                    booking.ParkingReservation
                        .FeeAtReservation =
                            parkingSlot.Fee;

                    booking.ParkingReservation
                        .ReservedAt =
                            utcNow;

                    booking.ParkingReservation
                        .ReleasedAt =
                            null;

                    booking.ParkingReservation
                        .IsActive =
                            true;

                    booking.ParkingReservation
                        .ParkingSlot =
                            parkingSlot;
                }

                parkingSlot.Status =
                    "Reserved";

                parkingSlot.UpdatedAt =
                    utcNow;

                booking.UpdatedAt =
                    utcNow;

                bool saved =
                    await _bookingRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction
                        .RollbackAsync();

                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Failed to add parking reservation.");
                }

                await transaction
                    .CommitAsync();

                Booking? updatedBooking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (updatedBooking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was updated but could not be retrieved.");
                }

                return ServiceResult<BookingResponseDto>
                    .Ok(
                        MapToBookingResponseDto(
                            updatedBooking),
                        "Parking added successfully.");
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "An error occurred while adding parking.");
            }
        }

        // ======================================
        // REMOVE PARKING
        // ======================================

        public async Task<ServiceResult<BookingResponseDto>>
            RemoveParkingAsync(
                int bookingId,
                int customerId)
        {
            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                Booking? booking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (booking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was not found.");
                }

                if (booking.CustomerId !=
                    customerId)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "You are not authorized to modify this booking.");
                }

                if (!string.Equals(
                    booking.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Parking can only be removed from a pending booking.");
                }

                DateTime utcNow =
                    DateTime.UtcNow;

                if (booking.HoldExpiresAt.HasValue &&
                    booking.HoldExpiresAt.Value <= utcNow)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking hold has expired.");
                }

                if (booking.ParkingReservation == null ||
                    !booking.ParkingReservation.IsActive)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking does not have an active parking reservation.");
                }

                booking.ParkingReservation
                    .IsActive =
                        false;

                booking.ParkingReservation
                    .ReleasedAt =
                        utcNow;

                if (booking.ParkingReservation
                    .ParkingSlot != null)
                {
                    booking.ParkingReservation
                        .ParkingSlot!
                        .Status =
                            "Available";

                    booking.ParkingReservation
                        .ParkingSlot!
                        .UpdatedAt =
                            utcNow;
                }

                booking.UpdatedAt =
                    utcNow;

                bool saved =
                    await _bookingRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction
                        .RollbackAsync();

                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Failed to remove parking reservation.");
                }

                await transaction
                    .CommitAsync();

                Booking? updatedBooking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (updatedBooking == null)
                {
                    return ServiceResult<BookingResponseDto>
                        .Fail(
                            "Booking was updated but could not be retrieved.");
                }

                BookingResponseDto response =
                    MapToBookingResponseDto(
                        updatedBooking);

                response.Parking =
                    null;

                response.ParkingFee =
                    0m;

                response.TotalAmount =
                    response.SeatTotal;

                return ServiceResult<BookingResponseDto>
                    .Ok(
                        response,
                        "Parking removed successfully.");
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "An error occurred while removing parking.");
            }
        }

        // ======================================
        // GET CUSTOMER BOOKINGS
        // ======================================

        public async Task<
            ServiceResult<List<BookingResponseDto>>>
            GetCustomerBookingsAsync(
                int customerId)
        {
            List<Booking> bookings =
                await _bookingRepository
                    .GetBookingsByCustomerAsync(
                        customerId);

            List<BookingResponseDto> response =
                bookings
                    .Select(
                        MapToBookingResponseDto)
                    .ToList();

            return ServiceResult<
                List<BookingResponseDto>>
                .Ok(
                    response,
                    "Bookings retrieved successfully.");
        }

        // ======================================
        // GET BOOKING BY ID
        // ======================================

        public async Task<ServiceResult<BookingResponseDto>>
            GetBookingByIdAsync(
                int bookingId,
                int customerId,
                bool isAdmin)
        {
            Booking? booking =
                await _bookingRepository
                    .GetBookingWithDetailsAsync(
                        bookingId);

            if (booking == null)
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "Booking was not found.");
            }

            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                return ServiceResult<BookingResponseDto>
                    .Fail(
                        "You are not authorized to access this booking.");
            }

            return ServiceResult<BookingResponseDto>
                .Ok(
                    MapToBookingResponseDto(
                        booking),
                    "Booking retrieved successfully.");
        }

        // ======================================
        // HOLD STATUS
        // ======================================

        public async Task<
            ServiceResult<BookingHoldStatusDto>>
            GetHoldStatusAsync(
                int bookingId,
                int customerId,
                bool isAdmin)
        {
            Booking? booking =
                await _bookingRepository
                    .GetBookingWithDetailsAsync(
                        bookingId);

            if (booking == null)
            {
                return ServiceResult<BookingHoldStatusDto>
                    .Fail(
                        "Booking was not found.");
            }

            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                return ServiceResult<BookingHoldStatusDto>
                    .Fail(
                        "You are not authorized to access this booking.");
            }

            DateTime utcNow =
                DateTime.UtcNow;

            bool isExpired =
                string.Equals(
                    booking.Status,
                    "Expired",
                    StringComparison.OrdinalIgnoreCase)
                ||
                (
                    string.Equals(
                        booking.Status,
                        "Pending",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    booking.HoldExpiresAt.HasValue
                    &&
                    booking.HoldExpiresAt.Value <= utcNow
                );

            int remainingSeconds =
                0;

            if (!isExpired &&
                string.Equals(
                    booking.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase)
                &&
                booking.HoldExpiresAt.HasValue)
            {
                remainingSeconds =
                    Math.Max(
                        0,
                        (int)Math.Ceiling(
                            (
                                booking.HoldExpiresAt.Value
                                - utcNow
                            ).TotalSeconds));
            }

            BookingHoldStatusDto response =
                new BookingHoldStatusDto
                {
                    BookingId =
                        booking.BookingId,

                    BookingNumber =
                        booking.BookingNumber,

                    Status =
                        isExpired &&
                        string.Equals(
                            booking.Status,
                            "Pending",
                            StringComparison.OrdinalIgnoreCase)
                            ? "Expired"
                            : booking.Status,

                    HoldExpiresAt =
                        booking.HoldExpiresAt,

                    RemainingSeconds =
                        remainingSeconds,

                    IsExpired =
                        isExpired
                };

            return ServiceResult<
                BookingHoldStatusDto>
                .Ok(
                    response,
                    "Booking hold status retrieved successfully.");
        }

        // ======================================
        // CANCEL BOOKING
        // ======================================

        public async Task<ServiceResult<bool>>
            CancelBookingAsync(
                int bookingId,
                int customerId,
                bool isAdmin)
        {
            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                Booking? booking =
                    await _bookingRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (booking == null)
                {
                    return ServiceResult<bool>
                        .Fail(
                            "Booking was not found.");
                }

                if (!isAdmin &&
                    booking.CustomerId != customerId)
                {
                    return ServiceResult<bool>
                        .Fail(
                            "You are not authorized to cancel this booking.");
                }

                if (string.Equals(
                    booking.Status,
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<bool>
                        .Fail(
                            "Booking has already been cancelled.");
                }

                if (string.Equals(
                    booking.Status,
                    "Expired",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<bool>
                        .Fail(
                            "Expired booking cannot be cancelled.");
                }

                DateTime utcNow =
                    DateTime.UtcNow;

                booking.Status =
                    "Cancelled";

                booking.CancelledAt =
                    utcNow;

                booking.UpdatedAt =
                    utcNow;

                ReleaseResources(
                    booking,
                    utcNow);

                bool saved =
                    await _bookingRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction
                        .RollbackAsync();

                    return ServiceResult<bool>
                        .Fail(
                            "Failed to cancel booking.");
                }

                await transaction
                    .CommitAsync();

                await _notificationService
                    .CreateNotificationAsync(
                        new NotificationCreateDto
                        {
                            CustomerId =
                                booking.CustomerId,

                            BookingId =
                                booking.BookingId,

                            EventId =
                                booking.EventId,

                            Type =
                                "BookingCancelled",

                            Title =
                                "Booking Cancelled",

                            Message =
                                $"Booking {booking.BookingNumber} has been cancelled."
                        });

                return ServiceResult<bool>
                    .Ok(
                        true,
                        "Booking cancelled successfully.");
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                return ServiceResult<bool>
                    .Fail(
                        "An error occurred while cancelling the booking.");
            }
        }

        // ======================================
        // ADMIN BOOKING LIST
        // ======================================

        public async Task<
            ServiceResult<List<BookingResponseDto>>>
            GetBookingsAsync(
                int? eventId)
        {
            List<Booking> bookings =
                await _bookingRepository
                    .GetBookingsAsync(
                        eventId);

            List<BookingResponseDto> response =
                bookings
                    .Select(
                        MapToBookingResponseDto)
                    .ToList();

            return ServiceResult<
                List<BookingResponseDto>>
                .Ok(
                    response,
                    "Bookings retrieved successfully.");
        }

        // ======================================
        // EXPIRE PENDING BOOKINGS
        // ======================================

        public async Task
            ExpirePendingBookingsAsync()
        {
            DateTime utcNow =
                DateTime.UtcNow;

            await using var transaction =
                await _bookingRepository
                    .BeginTransactionAsync();

            try
            {
                List<Booking> expiredBookings =
                    await _bookingRepository
                        .GetExpiredPendingBookingsAsync(
                            utcNow);

                if (expiredBookings.Count == 0)
                {
                    await transaction
                        .CommitAsync();

                    return;
                }

                foreach (Booking booking
                         in expiredBookings)
                {
                    booking.Status =
                        "Expired";

                    booking.UpdatedAt =
                        utcNow;

                    ReleaseResources(
                        booking,
                        utcNow);
                }

                await _bookingRepository
                    .SaveChangesAsync();

                await transaction
                    .CommitAsync();
            }
            catch
            {
                await transaction
                    .RollbackAsync();

                throw;
            }
        }

        // ======================================
        // RELEASE SEATS + PARKING
        // ======================================

        private static void ReleaseResources(
            Booking booking,
            DateTime utcNow)
        {
            foreach (BookingSeat bookingSeat
                     in booking.BookingSeats)
            {
                if (!bookingSeat.IsActive)
                {
                    continue;
                }

                bookingSeat.IsActive =
                    false;

                bookingSeat.ReleasedAt =
                    utcNow;

                if (bookingSeat.Seat != null)
                {
                    bookingSeat.Seat.Status =
                        "Available";

                    bookingSeat.Seat.UpdatedAt =
                        utcNow;
                }
            }

            if (booking.ParkingReservation != null &&
                booking.ParkingReservation.IsActive)
            {
                booking.ParkingReservation.IsActive =
                    false;

                booking.ParkingReservation.ReleasedAt =
                    utcNow;

                if (booking.ParkingReservation
                    .ParkingSlot != null)
                {
                    booking.ParkingReservation
                        .ParkingSlot!
                        .Status =
                            "Available";

                    booking.ParkingReservation
                        .ParkingSlot!
                        .UpdatedAt =
                            utcNow;
                }
            }
        }

        // ======================================
        // BOOKING RESPONSE MAPPER
        // ======================================

        private static BookingResponseDto
            MapToBookingResponseDto(
                Booking booking)
        {
            List<BookingSeatDto> seatDtos =
                booking.BookingSeats
                    .Select(bookingSeat =>
                        new BookingSeatDto
                        {
                            SeatId =
                                bookingSeat.SeatId,

                            SeatNumber =
                                bookingSeat.Seat?
                                    .SeatNumber
                                ?? string.Empty,

                            RowName =
                                bookingSeat.Seat?
                                    .RowName,

                            ColumnNumber =
                                bookingSeat.Seat?
                                    .ColumnNumber,

                            SeatType =
                                bookingSeat.Seat?
                                    .SeatType,

                            PriceAtBooking =
                                bookingSeat
                                    .PriceAtBooking
                        })
                    .ToList();

            decimal seatTotal =
                booking.BookingSeats
                    .Sum(bookingSeat =>
                        bookingSeat.PriceAtBooking);

            BookingParkingDto? parkingDto =
                null;

            decimal parkingFee =
                0m;

            // Pending booking-la customer parking
            // remove pannina inactive parking-a
            // response-la kaatta koodathu.
            //
            // Cancelled / Expired history-la
            // old parking details preserve pannuvom.

            bool shouldIncludeParking =
                booking.ParkingReservation != null
                &&
                (
                    booking.ParkingReservation.IsActive
                    ||
                    !string.Equals(
                        booking.Status,
                        "Pending",
                        StringComparison.OrdinalIgnoreCase)
                );

            if (shouldIncludeParking)
            {
                parkingFee =
                    booking.ParkingReservation!
                        .FeeAtReservation;

                parkingDto =
                    new BookingParkingDto
                    {
                        ParkingSlotId =
                            booking.ParkingReservation!
                                .ParkingSlotId,

                        SlotNumber =
                            booking.ParkingReservation!
                                .ParkingSlot?
                                .SlotNumber
                            ?? string.Empty,

                        Zone =
                            booking.ParkingReservation!
                                .ParkingSlot?
                                .Zone,

                        FeeAtReservation =
                            booking.ParkingReservation!
                                .FeeAtReservation
                    };
            }

            return new BookingResponseDto
            {
                BookingId =
                    booking.BookingId,

                BookingNumber =
                    booking.BookingNumber,

                CustomerId =
                    booking.CustomerId,

                CustomerName =
                    booking.Customer?
                        .FullName
                    ?? string.Empty,

                EventId =
                    booking.EventId,

                EventName =
                    booking.Event?
                        .EventName
                    ?? string.Empty,

                Status =
                    booking.Status,

                HoldExpiresAt =
                    booking.HoldExpiresAt,

                Seats =
                    seatDtos,

                Parking =
                    parkingDto,

                SeatTotal =
                    seatTotal,

                ParkingFee =
                    parkingFee,

                TotalAmount =
                    seatTotal + parkingFee,

                CreatedAt =
                    booking.CreatedAt,

                ConfirmedAt =
                    booking.ConfirmedAt,

                CancelledAt =
                    booking.CancelledAt
            };
        }
    }
}