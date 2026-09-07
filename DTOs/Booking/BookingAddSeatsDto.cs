using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Booking
{
    public class BookingAddSeatsDto
    {
        [Required]
        [MinLength(
            1,
            ErrorMessage =
                "At least one seat must be selected."
        )]
        public List<int> SeatIds { get; set; }
            = new List<int>();
    }
}