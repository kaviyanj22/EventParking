using System.ComponentModel.DataAnnotations;

namespace Event_parking.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [MaxLength(150)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}