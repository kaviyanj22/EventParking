using Event_parking.Data;
using Event_parking.DTOs.Dashboard;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class AdminDashboardRepository
        : IAdminDashboardRepository
    {
        private readonly ApplicationDbContext
            _context;

        public AdminDashboardRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET ADMIN DASHBOARD STATISTICS
        // ======================================

        public async Task<AdminDashboardDto>
            GetDashboardStatisticsAsync()
        {
            int totalEvents =
                await _context.Events
                    .CountAsync();

            int totalBookings =
                await _context.Bookings
                    .CountAsync();

            int availableSeats =
                await _context.Seats
                    .CountAsync(seat =>
                        seat.Status == "Available");

            int occupiedParkingSlots =
                await _context.ParkingSlots
                    .CountAsync(parking =>
                        parking.Status == "Reserved"
                        ||
                        parking.Status == "Occupied");

            decimal totalRevenueCollected =
                await _context.Payments
                    .Where(payment =>
                        payment.Status == "Completed")
                    .SumAsync(payment =>
                        (decimal?)payment.Amount)
                ?? 0m;

            int totalCustomers =
                await _context.Customers
                    .CountAsync(customer =>
                        customer.Role == "Customer");

            return new AdminDashboardDto
            {
                TotalEvents =
                    totalEvents,

                TotalBookings =
                    totalBookings,

                AvailableSeats =
                    availableSeats,

                OccupiedParkingSlots =
                    occupiedParkingSlots,

                TotalRevenueCollected =
                    totalRevenueCollected,

                TotalCustomers =
                    totalCustomers
            };
        }
    }
}