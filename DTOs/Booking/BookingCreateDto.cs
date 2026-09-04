using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Booking
{
    public class BookingCreateDto
    {
        [Required]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "A valid event is required."
        )]
        public int EventId { get; set; }

        [Required]
        [MinLength(
            1,
            ErrorMessage = "At least one seat must be selected."
        )]
        public List<int> SeatIds { get; set; }
            = new List<int>();

        public int? ParkingSlotId { get; set; }
    }
}