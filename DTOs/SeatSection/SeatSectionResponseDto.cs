namespace Event_parking.DTOs.SeatSection
{
    public class SeatSectionResponseDto
    {
        public int SeatSectionId { get; set; }

        public int EventId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? LayoutImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        public int SeatCount { get; set; }
    }
}