using Event_parking.DTOs.Dashboard;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class AdminDashboardService
        : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository
            _adminDashboardRepository;

        public AdminDashboardService(
            IAdminDashboardRepository adminDashboardRepository)
        {
            _adminDashboardRepository =
                adminDashboardRepository;
        }

        // ======================================
        // GET ADMIN DASHBOARD
        // ======================================

        public async Task<ServiceResult<AdminDashboardDto>>
            GetDashboardAsync()
        {
            AdminDashboardDto dashboard =
                await _adminDashboardRepository
                    .GetDashboardStatisticsAsync();

            return ServiceResult<AdminDashboardDto>
                .Ok(
                    dashboard,
                    "Admin dashboard retrieved successfully.");
        }
    }
}