using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParkingSlot>>
            GetParkingSlotsByEventIdAsync(int eventId)
        {
            return await _context.ParkingSlots
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.Zone)
                .ThenBy(p => p.SlotNumber)
                .ToListAsync();
        }

        public async Task<ParkingSlot?>
            GetParkingSlotByIdAsync(int parkingSlotId)
        {
            return await _context.ParkingSlots
                .FirstOrDefaultAsync(
                    p => p.ParkingSlotId == parkingSlotId
                );
        }

        public async Task<bool> EventExistsAsync(int eventId)
        {
            return await _context.Events
                .AnyAsync(e => e.EventId == eventId);
        }

        public async Task<bool> SlotNumberExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeParkingSlotId = null)
        {
            IQueryable<ParkingSlot> query =
                _context.ParkingSlots
                    .Where(p =>
                        p.EventId == eventId &&
                        p.SlotNumber == slotNumber);

            if (excludeParkingSlotId.HasValue)
            {
                query = query.Where(
                    p => p.ParkingSlotId !=
                         excludeParkingSlotId.Value
                );
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveReservationAsync(
            int parkingSlotId)
        {
            return await _context.ParkingReservations
                .AnyAsync(pr =>
                    pr.ParkingSlotId == parkingSlotId &&
                    pr.IsActive);
        }

        public async Task AddParkingSlotAsync(
            ParkingSlot parkingSlot)
        {
            await _context.ParkingSlots
                .AddAsync(parkingSlot);
        }

        public async Task AddParkingSlotsAsync(
            IEnumerable<ParkingSlot> parkingSlots)
        {
            await _context.ParkingSlots
                .AddRangeAsync(parkingSlots);
        }

        public void UpdateParkingSlot(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots.Update(parkingSlot);
        }

        public void DeleteParkingSlot(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots.Remove(parkingSlot);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}