using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Notification
{
    public class NotificationCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        public int? BookingId { get; set; }

        public int? EventId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
            = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
            = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
            = string.Empty;
    }
}