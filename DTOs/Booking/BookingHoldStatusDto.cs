namespace Event_parking.DTOs.Booking
{
    public class BookingHoldStatusDto
    {
        public int BookingId { get; set; }

        public string BookingNumber { get; set; }
            = string.Empty;

        public string Status { get; set; }
            = string.Empty;

        public DateTime? HoldExpiresAt { get; set; }

        public int RemainingSeconds { get; set; }

        public bool IsExpired { get; set; }
    }
}