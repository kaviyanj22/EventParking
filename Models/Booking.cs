using System.ComponentModel.DataAnnotations;

namespace Event_parking.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(30)]
        public string BookingNumber { get; set; }
            = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
            = "Pending";

        public DateTime? HoldExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        // ======================================
        // NAVIGATION PROPERTIES
        // ======================================

        public Customer? Customer { get; set; }

        public Event? Event { get; set; }

        public ICollection<BookingSeat> BookingSeats
        {
            get;
            set;
        } = new List<BookingSeat>();

        public ParkingReservation?
            ParkingReservation
        {
            get;
            set;
        }

        public Payment? Payment { get; set; }
    }
}