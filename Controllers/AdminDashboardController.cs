using Event_parking.DTOs.Dashboard;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController
        : ControllerBase
    {
        private readonly IAdminDashboardService
            _adminDashboardService;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService =
                adminDashboardService;
        }

        // ======================================
        // GET ADMIN DASHBOARD
        // GET /api/admin/dashboard
        // ADMIN ONLY
        // ======================================

        [HttpGet]
        public async Task<IActionResult>
            GetDashboard()
        {
            ServiceResult<AdminDashboardDto> result =
                await _adminDashboardService
                    .GetDashboardAsync();

            return Ok(result);
        }
    }
}