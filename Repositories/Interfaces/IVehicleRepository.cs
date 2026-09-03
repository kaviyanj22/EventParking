using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>>
            GetByCustomerIdAsync(
                int customerId
            );

        Task<Vehicle?> GetOwnedVehicleAsync(
            int vehicleId,
            int customerId
        );

        Task<bool> VehicleNumberExistsAsync(
            string vehicleNumber,
            int? exceptVehicleId = null
        );

        Task AddAsync(Vehicle vehicle);

        void Delete(Vehicle vehicle);

        Task SaveChangesAsync();
    }
}