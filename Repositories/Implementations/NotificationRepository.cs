using Event_parking.Data;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET CUSTOMER NOTIFICATIONS
        // ======================================

        public async Task<List<Notification>>
            GetNotificationsByCustomerAsync(
                int customerId)
        {
            return await _context.Notifications

                .Where(notification =>
                    notification.CustomerId == customerId)

                .OrderByDescending(notification =>
                    notification.CreatedAt)

                .ToListAsync();
        }

        // ======================================
        // GET ONE NOTIFICATION
        // ======================================

        public async Task<Notification?>
            GetNotificationByIdAsync(
                int notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(notification =>
                    notification.NotificationId == notificationId);
        }

        // ======================================
        // ADD NOTIFICATION
        // ======================================

        public async Task AddNotificationAsync(
            Notification notification)
        {
            await _context.Notifications
                .AddAsync(notification);
        }

        // ======================================
        // SAVE
        // ======================================

        public async Task<bool> SaveChangesAsync()
        {
            return await _context
                .SaveChangesAsync() > 0;
        }
    }
}