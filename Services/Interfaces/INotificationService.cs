using Event_parking.DTOs.Notification;

namespace Event_parking.Services.Interfaces
{
    public interface INotificationService
    {
        // ======================================
        // INTERNAL CREATE
        // ======================================

        Task<ServiceResult<NotificationResponseDto>>
            CreateNotificationAsync(
                NotificationCreateDto dto
            );

        // ======================================
        // CUSTOMER NOTIFICATIONS
        // ======================================

        Task<ServiceResult<List<NotificationResponseDto>>>
            GetCustomerNotificationsAsync(
                int customerId
            );

        // ======================================
        // MARK AS READ
        // ======================================

        Task<ServiceResult<NotificationResponseDto>>
            MarkAsReadAsync(
                int notificationId,
                int customerId
            );
    }
}