using Event_parking.DTOs.Seat;

namespace Event_parking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatResponseDto>> GetSeatsByEventIdAsync(
            int eventId
        );

        Task<SeatResponseDto?> GetSeatByIdAsync(
            int eventId,
            int seatId
        );

        Task<(bool Success, string Message)> CreateSeatMapAsync(
            int eventId,
            SeatMapCreateDto dto
        );

        Task<(bool Success, string Message, SeatResponseDto? Data)>
            CreateSeatAsync(
                int eventId,
                SeatCreateDto dto
            );

        Task<(bool Success, string Message)> UpdateSeatAsync(
            int eventId,
            int seatId,
            SeatUpdateDto dto
        );

        Task<(bool Success, string Message)> DeleteSeatAsync(
            int eventId,
            int seatId
        );
    }
}