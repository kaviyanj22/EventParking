using Event_parking.DTOs.Dashboard;

namespace Event_parking.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        // ======================================
        // GET ADMIN DASHBOARD
        // ======================================

        Task<ServiceResult<AdminDashboardDto>>
            GetDashboardAsync();
    }
}