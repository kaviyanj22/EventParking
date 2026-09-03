using Event_parking.Data;
using Event_parking.DTOs.Event;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync(
            EventFilterDto? filter = null)
        {
            var query = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    query = query.Where(e =>
                        e.EventName.Contains(filter.Name));
                }

                if (filter.Date.HasValue)
                {
                    query = query.Where(e =>
                        e.EventDate.Date == filter.Date.Value.Date);
                }

                if (filter.VenueId.HasValue)
                {
                    query = query.Where(e =>
                        e.VenueId == filter.VenueId.Value);
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(e =>
                        e.CategoryId == filter.CategoryId.Value);
                }
            }

            return await query
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<Event> CreateAsync(Event eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();

            return eventEntity;
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();

            return eventEntity;
        }

        public async Task<bool> DeleteAsync(Event eventEntity)
        {
            _context.Events.Remove(eventEntity);

            return await _context.SaveChangesAsync() > 0;
        }

        public Task<bool> HasActiveBookingsAsync(int eventId)
        {
            // Booking module is not implemented yet.
            // Replace this after Booking integration.
            return Task.FromResult(false);
        }

        public Task<int> GetBookedSeatCountAsync(int eventId)
        {
            // BookingSeat module is not implemented yet.
            // Replace this after Booking integration.
            return Task.FromResult(0);
        }

        public async Task<bool> HasVenueOverlapAsync(
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
                query = query.Where(e =>
                    e.EventId != excludeEventId.Value);
            }

            return await query.AnyAsync(e =>
                startTime < e.EndTime &&
                endTime > e.StartTime);
        }
    }
}