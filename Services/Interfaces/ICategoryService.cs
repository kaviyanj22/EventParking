using Event_parking.DTOs.Category;

namespace Event_parking.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();

        Task<CategoryResponseDto?> GetByIdAsync(int id);

        Task<CategoryResponseDto> CreateAsync(
            CategoryCreateDto createDto);

        Task<CategoryResponseDto?> UpdateAsync(
            int id,
            CategoryUpdateDto updateDto);

        Task<bool> DeleteAsync(int id);
    }
}
