namespace Event_parking.DTOs.Booking
{
    public class BookingSeatDto
    {
        public int SeatId { get; set; }

        public string SeatNumber { get; set; }
            = string.Empty;

        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        public string? SeatType { get; set; }

        public decimal PriceAtBooking { get; set; }
    }
}