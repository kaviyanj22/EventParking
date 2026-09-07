namespace Event_parking.DTOs.Seat
{
    public class SeatResponseDto
    {
        public int SeatId { get; set; }

        public int EventId { get; set; }

        public int? SeatSectionId { get; set; }

        public string? SectionName { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        public decimal? PositionX { get; set; }

        public decimal? PositionY { get; set; }

        public string? SeatType { get; set; }

        public decimal? Price { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}