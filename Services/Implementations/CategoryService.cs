using Event_parking.DTOs.Category;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(MapToResponseDto);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return MapToResponseDto(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(
            CategoryCreateDto createDto)
        {
            var categoryName = createDto.CategoryName.Trim();

            var existingCategory =
                await _categoryRepository.GetByNameAsync(categoryName);

            if (existingCategory != null)
            {
                throw new InvalidOperationException(
                    "A category with this name already exists.");
            }

            var category = new EventCategory
            {
                CategoryName = categoryName,
                Description = createDto.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var createdCategory =
                await _categoryRepository.CreateAsync(category);

            return MapToResponseDto(createdCategory);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(
            int id,
            CategoryUpdateDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            var categoryName = updateDto.CategoryName.Trim();

            var existingCategory =
                await _categoryRepository.GetByNameAsync(categoryName);

            if (existingCategory != null &&
                existingCategory.CategoryId != id)
            {
                throw new InvalidOperationException(
                    "A category with this name already exists.");
            }

            category.CategoryName = categoryName;
            category.Description = updateDto.Description?.Trim();
            category.UpdatedAt = DateTime.UtcNow;

            var updatedCategory =
                await _categoryRepository.UpdateAsync(category);

            return MapToResponseDto(updatedCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            var isInUse =
                await _categoryRepository.IsCategoryInUseAsync(id);

            if (isInUse)
            {
                throw new InvalidOperationException(
                    "This category cannot be deleted because it is assigned to one or more events.");
            }

            return await _categoryRepository.DeleteAsync(category);
        }

        private static CategoryResponseDto MapToResponseDto(
            EventCategory category)
        {
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}