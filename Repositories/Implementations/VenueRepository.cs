using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _context;

        public VenueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .AsNoTracking()
                .OrderBy(v => v.VenueName)
                .ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VenueId == id);
        }

        public async Task<Venue> CreateAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();

            return venue;
        }

        public async Task<Venue> UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();

            return venue;
        }

        public async Task<bool> DeleteAsync(Venue venue)
        {
            _context.Venues.Remove(venue);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasUpcomingEventsAsync(int venueId)
        {
            var today = DateTime.Today;

            return await _context.Events
                .AnyAsync(e =>
                    e.VenueId == venueId &&
                    e.EventDate.Date >= today);
        }

        public async Task<bool> IsAvailableAsync(
            int venueId,
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeEventId = null)
        {
            var query = _context.Events
                .Where(e =>
                    e.VenueId == venueId &&
                    e.EventDate.Date == eventDate.Date);

            if (excludeEventId.HasValue)
            {
                query = query.Where(
                    e => e.EventId != excludeEventId.Value);
            }

            bool hasOverlap = await query.AnyAsync(e =>
                startTime < e.EndTime &&
                endTime > e.StartTime);

            return !hasOverlap;
        }

        public async Task<IEnumerable<Venue>> GetAvailableVenuesAsync(
            DateTime eventDate,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var unavailableVenueIds = await _context.Events
                .Where(e =>
                    e.EventDate.Date == eventDate.Date &&
                    startTime < e.EndTime &&
                    endTime > e.StartTime)
                .Select(e => e.VenueId)
                .Distinct()
                .ToListAsync();

            return await _context.Venues
                .AsNoTracking()
                .Where(v => !unavailableVenueIds.Contains(v.VenueId))
                .OrderBy(v => v.VenueName)
                .ToListAsync();
        }
    }
}