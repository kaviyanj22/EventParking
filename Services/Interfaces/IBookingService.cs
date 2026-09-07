using Event_parking.DTOs.Booking;

namespace Event_parking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResult<BookingResponseDto>>
            CreateBookingAsync(
                int customerId,
                BookingCreateDto dto
            );

        Task<ServiceResult<BookingResponseDto>>
            AddSeatsAsync(
                int bookingId,
                int customerId,
                BookingAddSeatsDto dto
            );

        Task<ServiceResult<BookingResponseDto>>
            AddParkingAsync(
                int bookingId,
                int customerId,
                BookingParkingRequestDto dto
            );

        Task<ServiceResult<BookingResponseDto>>
            RemoveParkingAsync(
                int bookingId,
                int customerId
            );

        Task<ServiceResult<List<BookingResponseDto>>>
            GetCustomerBookingsAsync(
                int customerId
            );

        Task<ServiceResult<BookingResponseDto>>
            GetBookingByIdAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        Task<ServiceResult<BookingHoldStatusDto>>
            GetHoldStatusAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        Task<ServiceResult<bool>>
            CancelBookingAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        Task<ServiceResult<List<BookingResponseDto>>>
            GetBookingsAsync(
                int? eventId
            );

        Task ExpirePendingBookingsAsync();
    }
}