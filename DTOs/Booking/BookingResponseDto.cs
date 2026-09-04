namespace Event_parking.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }

        public string BookingNumber { get; set; }
            = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
            = string.Empty;

        public int EventId { get; set; }

        public string EventName { get; set; }
            = string.Empty;

        public string Status { get; set; }
            = string.Empty;

        public DateTime? HoldExpiresAt { get; set; }

        public List<BookingSeatDto> Seats { get; set; }
            = new List<BookingSeatDto>();

        public BookingParkingDto? Parking { get; set; }

        public decimal SeatTotal { get; set; }

        public decimal ParkingFee { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }
}