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

        public int? SeatSectionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        // Custom visual position inside the section/layout
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PositionX { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PositionY { get; set; }

        [MaxLength(50)]
        public string? SeatType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        [ForeignKey(nameof(SeatSectionId))]
        public SeatSection? SeatSection { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}