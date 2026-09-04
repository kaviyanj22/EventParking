namespace Event_parking.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        public string BookingNumber { get; set; }
            = string.Empty;

        public int CustomerId { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }
            = string.Empty;

        public string? TransactionReference { get; set; }

        public DateTime PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}