using Event_parking.DTOs.Dashboard;

namespace Event_parking.Repositories.Interfaces
{
    public interface IAdminDashboardRepository
    {
        // ======================================
        // GET ADMIN DASHBOARD STATISTICS
        // ======================================

        Task<AdminDashboardDto>
            GetDashboardStatisticsAsync();
    }
}