using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_parking.Models
{
    public class SeatSection
    {
        [Key]
        public int SeatSectionId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SectionName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? LayoutImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public ICollection<Seat> Seats { get; set; }
            = new List<Seat>();
    }
}