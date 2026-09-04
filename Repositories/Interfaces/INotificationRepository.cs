using Event_parking.Models;

namespace Event_parking.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        // ======================================
        // GET CUSTOMER NOTIFICATIONS
        // ======================================

        Task<List<Notification>>
            GetNotificationsByCustomerAsync(
                int customerId
            );

        // ======================================
        // GET ONE NOTIFICATION
        // ======================================

        Task<Notification?>
            GetNotificationByIdAsync(
                int notificationId
            );

        // ======================================
        // CREATE NOTIFICATION
        // ======================================

        Task AddNotificationAsync(
            Notification notification
        );

        // ======================================
        // SAVE
        // ======================================

        Task<bool> SaveChangesAsync();
    }
}