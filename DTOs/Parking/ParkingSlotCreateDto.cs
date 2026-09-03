using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Parking
{
    public class ParkingSlotCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string SlotNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Zone { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Parking fee cannot be negative.")]
        public decimal Fee { get; set; }
    }
}