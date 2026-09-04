using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class ParkingSlot
    {
        [Key]
        public int ParkingSlotId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SlotNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Zone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public Event? Event { get; set; }

        public ICollection<ParkingReservation> ParkingReservations { get; set; }
            = new List<ParkingReservation>();
    }
}