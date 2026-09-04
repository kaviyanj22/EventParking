namespace Event_parking.DTOs.Payment
{
    public class ReceiptDto
    {
        public int PaymentId { get; set; }

        public string TransactionReference { get; set; }
            = string.Empty;

        public int BookingId { get; set; }

        public string BookingNumber { get; set; }
            = string.Empty;

        public string CustomerName { get; set; }
            = string.Empty;

        public string CustomerEmail { get; set; }
            = string.Empty;

        public string EventName { get; set; }
            = string.Empty;

        public decimal SeatTotal { get; set; }

        public decimal ParkingFee { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; }
            = string.Empty;

        public DateTime PaidAt { get; set; }
    }
}