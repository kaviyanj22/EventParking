namespace Event_parking.DTOs.Payment
{
    public class PaymentSummaryDto
    {
        public int BookingId { get; set; }

        public string BookingNumber { get; set; }
            = string.Empty;

        public string EventName { get; set; }
            = string.Empty;

        public decimal SeatTotal { get; set; }

        public decimal ParkingFee { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; }
            = string.Empty;

        public string PaymentStatus { get; set; }
            = string.Empty;

        public DateTime? HoldExpiresAt { get; set; }

        public int RemainingSeconds { get; set; }

        public bool IsExpired { get; set; }
    }
}