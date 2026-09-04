using Event_parking.DTOs.Notification;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository
            _notificationRepository;

        public NotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository =
                notificationRepository;
        }

        // ======================================
        // CREATE NOTIFICATION
        // INTERNAL USE
        // ======================================

        public async Task<
            ServiceResult<NotificationResponseDto>>
            CreateNotificationAsync(
                NotificationCreateDto dto)
        {
            if (dto.CustomerId <= 0)
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "A valid customer is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Notification type is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Notification title is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Notification message is required.");
            }

            Notification notification =
                new Notification
                {
                    CustomerId = dto.CustomerId,
                    BookingId = dto.BookingId,
                    EventId = dto.EventId,
                    Type = dto.Type.Trim(),
                    Title = dto.Title.Trim(),
                    Message = dto.Message.Trim(),
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

            await _notificationRepository
                .AddNotificationAsync(notification);

            bool saved =
                await _notificationRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Failed to create notification.");
            }

            NotificationResponseDto response =
                MapToResponseDto(notification);

            return ServiceResult<
                NotificationResponseDto>.Ok(
                    response,
                    "Notification created successfully.");
        }

        // ======================================
        // GET CUSTOMER NOTIFICATIONS
        // ======================================

        public async Task<
            ServiceResult<List<NotificationResponseDto>>>
            GetCustomerNotificationsAsync(
                int customerId)
        {
            List<Notification> notifications =
                await _notificationRepository
                    .GetNotificationsByCustomerAsync(
                        customerId);

            List<NotificationResponseDto> response =
                notifications
                    .Select(MapToResponseDto)
                    .ToList();

            return ServiceResult<
                List<NotificationResponseDto>>.Ok(
                    response,
                    "Notifications retrieved successfully.");
        }

        // ======================================
        // MARK NOTIFICATION AS READ
        // ======================================

        public async Task<
            ServiceResult<NotificationResponseDto>>
            MarkAsReadAsync(
                int notificationId,
                int customerId)
        {
            Notification? notification =
                await _notificationRepository
                    .GetNotificationByIdAsync(
                        notificationId);

            if (notification == null)
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Notification was not found.");
            }

            // Customer can update only own notification
            if (notification.CustomerId != customerId)
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "You are not authorized to access this notification.");
            }

            // Already read
            if (notification.IsRead)
            {
                return ServiceResult<
                    NotificationResponseDto>.Ok(
                        MapToResponseDto(notification),
                        "Notification is already marked as read.");
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            bool saved =
                await _notificationRepository
                    .SaveChangesAsync();

            if (!saved)
            {
                return ServiceResult<
                    NotificationResponseDto>.Fail(
                        "Failed to update notification.");
            }

            return ServiceResult<
                NotificationResponseDto>.Ok(
                    MapToResponseDto(notification),
                    "Notification marked as read successfully.");
        }

        // ======================================
        // MAPPER
        // ======================================

        private static NotificationResponseDto
            MapToResponseDto(
                Notification notification)
        {
            return new NotificationResponseDto
            {
                NotificationId =
                    notification.NotificationId,

                CustomerId =
                    notification.CustomerId,

                BookingId =
                    notification.BookingId,

                EventId =
                    notification.EventId,

                Type =
                    notification.Type,

                Title =
                    notification.Title,

                Message =
                    notification.Message,

                IsRead =
                    notification.IsRead,

                CreatedAt =
                    notification.CreatedAt,

                ReadAt =
                    notification.ReadAt
            };
        }
    }
}