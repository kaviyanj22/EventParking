namespace Event_parking.DTOs.Venue
{
    public class VenueResponseDto
    {
        public int VenueId { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}