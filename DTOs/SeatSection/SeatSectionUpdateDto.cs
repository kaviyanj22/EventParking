using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.SeatSection
{
    public class SeatSectionUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string SectionName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? LayoutImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }
}