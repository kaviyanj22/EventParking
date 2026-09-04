using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class BookingSeat
    {
        [Key]
        public int BookingSeatId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtBooking { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime ReservedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReleasedAt { get; set; }

        // Navigation to existing Member 3 Seat
        public Seat? Seat { get; set; }
    }
}