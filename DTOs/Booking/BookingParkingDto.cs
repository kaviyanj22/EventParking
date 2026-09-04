namespace Event_parking.DTOs.Booking
{
    public class BookingParkingDto
    {
        public int ParkingSlotId { get; set; }

        public string SlotNumber { get; set; }
            = string.Empty;

        public string? Zone { get; set; }

        public decimal FeeAtReservation { get; set; }
    }
}