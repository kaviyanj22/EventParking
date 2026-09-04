using System.ComponentModel.DataAnnotations;

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

        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReleasedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Booking? Booking { get; set; }

        public ParkingSlot? ParkingSlot { get; set; }
    }
}
