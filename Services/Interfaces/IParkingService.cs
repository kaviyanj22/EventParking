using Event_parking.DTOs.Parking;

namespace Event_parking.Services.Interfaces
{
    public interface IParkingService
    {
        Task<IEnumerable<ParkingSlotResponseDto>>
            GetParkingSlotsByEventIdAsync(
                int eventId
            );

        Task<ParkingSlotResponseDto?>
            GetParkingSlotByIdAsync(
                int eventId,
                int parkingSlotId
            );

        Task<(bool Success, string Message)>
            CreateParkingLayoutAsync(
                int eventId,
                ParkingLayoutCreateDto dto
            );

        Task<(bool Success, string Message, ParkingSlotResponseDto? Data)>
            CreateParkingSlotAsync(
                int eventId,
                ParkingSlotCreateDto dto
            );

        Task<(bool Success, string Message)>
            UpdateParkingSlotAsync(
                int eventId,
                int parkingSlotId,
                ParkingSlotUpdateDto dto
            );

        Task<(bool Success, string Message)>
            DeleteParkingSlotAsync(
                int eventId,
                int parkingSlotId
            );
    }
}