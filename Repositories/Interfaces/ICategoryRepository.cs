using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetAllAsync();

        Task<EventCategory?> GetByIdAsync(int id);

        Task<EventCategory?> GetByNameAsync(string categoryName);

        Task<EventCategory> CreateAsync(EventCategory category);

        Task<EventCategory> UpdateAsync(EventCategory category);

        Task<bool> DeleteAsync(EventCategory category);

        Task<bool> IsCategoryInUseAsync(int categoryId);
    }
}