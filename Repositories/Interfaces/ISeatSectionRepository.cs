using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface ISeatSectionRepository
    {
        Task<List<SeatSection>> GetByEventIdAsync(int eventId);

        Task<SeatSection?> GetByIdAsync(
            int eventId,
            int seatSectionId
        );

        Task<bool> ExistsByNameAsync(
            int eventId,
            string sectionName,
            int? excludeSeatSectionId = null
        );

        Task AddAsync(SeatSection seatSection);

        void Update(SeatSection seatSection);

        void Delete(SeatSection seatSection);

        Task SaveChangesAsync();
    }
}