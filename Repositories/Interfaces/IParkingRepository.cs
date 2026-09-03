using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface IParkingRepository
    {
        Task<IEnumerable<ParkingSlot>> GetParkingSlotsByEventIdAsync(
            int eventId
        );

        Task<ParkingSlot?> GetParkingSlotByIdAsync(
            int parkingSlotId
        );

        Task<bool> EventExistsAsync(
            int eventId
        );

        Task<bool> SlotNumberExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeParkingSlotId = null
        );

        Task<bool> HasActiveReservationAsync(
            int parkingSlotId
        );

        Task AddParkingSlotAsync(
            ParkingSlot parkingSlot
        );

        Task AddParkingSlotsAsync(
            IEnumerable<ParkingSlot> parkingSlots
        );

        void UpdateParkingSlot(
            ParkingSlot parkingSlot
        );

        void DeleteParkingSlot(
            ParkingSlot parkingSlot
        );

        Task<bool> SaveChangesAsync();
    }
}