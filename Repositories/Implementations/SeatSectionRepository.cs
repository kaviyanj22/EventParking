using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class SeatSectionRepository : ISeatSectionRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatSectionRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SeatSection>> GetByEventIdAsync(
            int eventId)
        {
            return await _context.SeatSections
                .Include(section => section.Seats)
                .Where(section =>
                    section.EventId == eventId)
                .OrderBy(section =>
                    section.DisplayOrder)
                .ThenBy(section =>
                    section.SectionName)
                .ToListAsync();
        }

        public async Task<SeatSection?> GetByIdAsync(
            int eventId,
            int seatSectionId)
        {
            return await _context.SeatSections
                .Include(section => section.Seats)
                .FirstOrDefaultAsync(section =>
                    section.EventId == eventId
                    &&
                    section.SeatSectionId ==
                        seatSectionId);
        }

        public async Task<bool> ExistsByNameAsync(
            int eventId,
            string sectionName,
            int? excludeSeatSectionId = null)
        {
            IQueryable<SeatSection> query =
                _context.SeatSections
                    .Where(section =>
                        section.EventId == eventId
                        &&
                        section.SectionName ==
                            sectionName);

            if (excludeSeatSectionId.HasValue)
            {
                query = query.Where(section =>
                    section.SeatSectionId !=
                    excludeSeatSectionId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(
            SeatSection seatSection)
        {
            await _context.SeatSections
                .AddAsync(seatSection);
        }

        public void Update(
            SeatSection seatSection)
        {
            _context.SeatSections
                .Update(seatSection);
        }

        public void Delete(
            SeatSection seatSection)
        {
            _context.SeatSections
                .Remove(seatSection);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}