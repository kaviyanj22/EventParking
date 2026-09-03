using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCategory>> GetAllAsync()
        {
            return await _context.EventCategories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<EventCategory?> GetByIdAsync(int id)
        {
            return await _context.EventCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<EventCategory?> GetByNameAsync(string categoryName)
        {
            return await _context.EventCategories
                .FirstOrDefaultAsync(c =>
                    c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<EventCategory> CreateAsync(EventCategory category)
        {
            await _context.EventCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<EventCategory> UpdateAsync(EventCategory category)
        {
            _context.EventCategories.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteAsync(EventCategory category)
        {
            _context.EventCategories.Remove(category);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsCategoryInUseAsync(int categoryId)
        {
            return await _context.Events
                .AnyAsync(e => e.CategoryId == categoryId);
        }
    }
}