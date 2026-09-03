using Event_parking.DTOs.Vehicle;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository
            _vehicleRepository;

        public VehicleService(
            IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<List<VehicleResponseDto>>
            GetMyVehiclesAsync(int customerId)
        {
            List<Vehicle> vehicles =
                await _vehicleRepository
                    .GetByCustomerIdAsync(customerId);

            return vehicles
                .Select(MapVehicle)
                .ToList();
        }

        public async Task<
            ServiceResult<VehicleResponseDto>>
            CreateVehicleAsync(
                int customerId,
                VehicleCreateDto createDto)
        {
            string vehicleNumber =
                createDto.VehicleNumber
                    .Trim()
                    .ToUpper();

            bool numberExists =
                await _vehicleRepository
                    .VehicleNumberExistsAsync(
                        vehicleNumber
                    );

            if (numberExists)
            {
                return ServiceResult<VehicleResponseDto>
                    .Fail(
                        "Vehicle number already exists."
                    );
            }

            var vehicle = new Vehicle
            {
                CustomerId = customerId,

                VehicleType =
                    createDto.VehicleType.Trim(),

                VehicleNumber = vehicleNumber,

                Make = createDto.Make?.Trim(),

                Model = createDto.Model?.Trim(),

                Color = createDto.Color?.Trim()
            };

            await _vehicleRepository.AddAsync(vehicle);

            await _vehicleRepository.SaveChangesAsync();

            return ServiceResult<VehicleResponseDto>
                .Ok(
                    MapVehicle(vehicle),
                    "Vehicle added successfully."
                );
        }

        public async Task<
            ServiceResult<VehicleResponseDto>>
            UpdateVehicleAsync(
                int customerId,
                int vehicleId,
                VehicleUpdateDto updateDto)
        {
            Vehicle? vehicle =
                await _vehicleRepository
                    .GetOwnedVehicleAsync(
                        vehicleId,
                        customerId
                    );

            if (vehicle == null)
            {
                return ServiceResult<VehicleResponseDto>
                    .Fail("Vehicle not found.");
            }

            string vehicleNumber =
                updateDto.VehicleNumber
                    .Trim()
                    .ToUpper();

            bool numberExists =
                await _vehicleRepository
                    .VehicleNumberExistsAsync(
                        vehicleNumber,
                        vehicleId
                    );

            if (numberExists)
            {
                return ServiceResult<VehicleResponseDto>
                    .Fail(
                        "Vehicle number already exists."
                    );
            }

            vehicle.VehicleType =
                updateDto.VehicleType.Trim();

            vehicle.VehicleNumber = vehicleNumber;

            vehicle.Make = updateDto.Make?.Trim();

            vehicle.Model = updateDto.Model?.Trim();

            vehicle.Color = updateDto.Color?.Trim();

            await _vehicleRepository.SaveChangesAsync();

            return ServiceResult<VehicleResponseDto>
                .Ok(
                    MapVehicle(vehicle),
                    "Vehicle updated successfully."
                );
        }

        public async Task<ServiceResult<object>>
            DeleteVehicleAsync(
                int customerId,
                int vehicleId)
        {
            Vehicle? vehicle =
                await _vehicleRepository
                    .GetOwnedVehicleAsync(
                        vehicleId,
                        customerId
                    );

            if (vehicle == null)
            {
                return ServiceResult<object>
                    .Fail("Vehicle not found.");
            }

            _vehicleRepository.Delete(vehicle);

            await _vehicleRepository.SaveChangesAsync();

            return ServiceResult<object>.Ok(
                null,
                "Vehicle deleted successfully."
            );
        }

        private static VehicleResponseDto MapVehicle(
            Vehicle vehicle)
        {
            return new VehicleResponseDto
            {
                VehicleId = vehicle.VehicleId,

                CustomerId = vehicle.CustomerId,

                VehicleType =
                    vehicle.VehicleType,

                VehicleNumber =
                    vehicle.VehicleNumber,

                Make = vehicle.Make,

                Model = vehicle.Model,

                Color = vehicle.Color
            };
        }
    }
}