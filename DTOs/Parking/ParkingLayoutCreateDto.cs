using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Parking
{
    public class ParkingLayoutCreateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one parking slot is required.")]
        public List<ParkingSlotCreateDto> ParkingSlots { get; set; } = new();
    }
}
