using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Seat
{
    public class SeatMapCreateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one seat is required.")]
        public List<SeatCreateDto> Seats { get; set; } = new();
    }
}