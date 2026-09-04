using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class ParkingReservation
    {
        [Key]
        public int ParkingReservationId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ParkingSlotId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FeeAtReservation { get; set; }

        public DateTime ReservedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReleasedAt { get; set; }

        public bool IsActive { get; set; }
            = true;

        // ======================================
        // NAVIGATION PROPERTIES
        // ======================================

        public Booking? Booking { get; set; }

        public ParkingSlot? ParkingSlot { get; set; }
    }
}