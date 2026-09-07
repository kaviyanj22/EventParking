using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Booking
{
    public class BookingParkingRequestDto
    {
        [Required]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "A valid parking slot is required."
        )]
        public int ParkingSlotId { get; set; }
    }
}