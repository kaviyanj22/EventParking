namespace Event_parking.DTOs.Event
{
    public class EventFilterDto
    {
        public string? Name { get; set; }

        public DateTime? Date { get; set; }

        public int? VenueId { get; set; }

        public int? CategoryId { get; set; }
    }
}