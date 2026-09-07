using Event_parking.DTOs.SeatSection;

namespace Event_parking.Services.Interfaces
{
    public interface ISeatSectionService
    {
        Task<ServiceResult<List<SeatSectionResponseDto>>>
            GetByEventIdAsync(int eventId);

        Task<ServiceResult<SeatSectionResponseDto>>
            GetByIdAsync(
                int eventId,
                int seatSectionId
            );

        Task<ServiceResult<SeatSectionResponseDto>>
            CreateAsync(
                int eventId,
                SeatSectionCreateDto dto
            );

        Task<ServiceResult<SeatSectionResponseDto>>
            UpdateAsync(
                int eventId,
                int seatSectionId,
                SeatSectionUpdateDto dto
            );

        Task<ServiceResult<bool>>
            DeleteAsync(
                int eventId,
                int seatSectionId
            );
    }
}