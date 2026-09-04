namespace Event_parking.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }

        public int CustomerId { get; set; }

        public int? BookingId { get; set; }

        public int? EventId { get; set; }

        public string Type { get; set; }
            = string.Empty;

        public string Title { get; set; }
            = string.Empty;

        public string Message { get; set; }
            = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}