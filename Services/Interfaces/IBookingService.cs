using Event_parking.DTOs.Booking;

namespace Event_parking.Services.Interfaces
{
    public interface IBookingService
    {
        // ======================================
        // CREATE BOOKING
        // ======================================

        Task<ServiceResult<BookingResponseDto>>
            CreateBookingAsync(
                int customerId,
                BookingCreateDto dto
            );

        // ======================================
        // GET CUSTOMER BOOKINGS
        // ======================================

        Task<ServiceResult<List<BookingResponseDto>>>
            GetCustomerBookingsAsync(
                int customerId
            );

        // ======================================
        // GET BOOKING BY ID
        // ======================================

        Task<ServiceResult<BookingResponseDto>>
            GetBookingByIdAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        // ======================================
        // HOLD STATUS
        // ======================================

        Task<ServiceResult<BookingHoldStatusDto>>
            GetHoldStatusAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        // ======================================
        // CANCEL BOOKING
        // ======================================

        Task<ServiceResult<bool>>
            CancelBookingAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        // ======================================
        // ADMIN BOOKING LIST
        // ======================================

        Task<ServiceResult<List<BookingResponseDto>>>
            GetBookingsAsync(
                int? eventId
            );

        // ======================================
        // EXPIRE PENDING BOOKINGS
        // ======================================

        Task ExpirePendingBookingsAsync();
    }
}