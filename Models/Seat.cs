using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        [MaxLength(50)]
        public string? SeatType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public Event? Event { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}