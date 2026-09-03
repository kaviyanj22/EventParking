using Event_parking.DTOs.Vehicle;

namespace Event_parking.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleResponseDto>>
            GetMyVehiclesAsync(
                int customerId
            );

        Task<ServiceResult<VehicleResponseDto>>
            CreateVehicleAsync(
                int customerId,
                VehicleCreateDto createDto
            );

        Task<ServiceResult<VehicleResponseDto>>
            UpdateVehicleAsync(
                int customerId,
                int vehicleId,
                VehicleUpdateDto updateDto
            );

        Task<ServiceResult<object>>
            DeleteVehicleAsync(
                int customerId,
                int vehicleId
            );
    }
}