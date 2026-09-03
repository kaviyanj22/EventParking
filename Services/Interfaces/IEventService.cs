using Event_parking.DTOs.Event;

namespace Event_parking.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponseDto>> GetAllAsync(
            EventFilterDto? filter = null);

        Task<EventResponseDto?> GetByIdAsync(int id);

        Task<EventResponseDto> CreateAsync(
            EventCreateDto createDto);

        Task<EventResponseDto?> UpdateAsync(
            int id,
            EventUpdateDto updateDto);

        Task<bool> DeleteAsync(int id);
    }
}