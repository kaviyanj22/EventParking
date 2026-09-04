using System.ComponentModel.DataAnnotations;

namespace Event_parking.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

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

        public bool IsRead { get; set; }
            = false;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // ======================================
        // NAVIGATION PROPERTIES
        // ======================================

        public Customer? Customer { get; set; }

        public Booking? Booking { get; set; }

        public Event? Event { get; set; }
    }
}