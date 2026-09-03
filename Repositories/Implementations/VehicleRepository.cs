using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class VehicleRepository
        : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>>
            GetByCustomerIdAsync(
                int customerId)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .Where(
                    vehicle =>
                        vehicle.CustomerId == customerId
                )
                .OrderBy(
                    vehicle => vehicle.VehicleNumber
                )
                .ToListAsync();
        }

        public async Task<Vehicle?>
            GetOwnedVehicleAsync(
                int vehicleId,
                int customerId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(
                    vehicle =>
                        vehicle.VehicleId == vehicleId
                        &&
                        vehicle.CustomerId == customerId
                );
        }

        public async Task<bool>
            VehicleNumberExistsAsync(
                string vehicleNumber,
                int? exceptVehicleId = null)
        {
            return await _context.Vehicles
                .AnyAsync(
                    vehicle =>
                        vehicle.VehicleNumber
                            == vehicleNumber
                        &&
                        (
                            !exceptVehicleId.HasValue
                            ||
                            vehicle.VehicleId
                                != exceptVehicleId.Value
                        )
                );
        }

        public async Task AddAsync(
            Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}