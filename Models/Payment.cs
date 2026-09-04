using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; }
            = "Completed";

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        public DateTime PaidAt { get; set; }
            = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // ======================================
        // NAVIGATION PROPERTIES
        // ======================================

        public Booking? Booking { get; set; }

        public Customer? Customer { get; set; }
    }
}