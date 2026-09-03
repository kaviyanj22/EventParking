using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Vehicle
{
    public class VehicleCreateDto
    {
        [Required]
        [MaxLength(30)]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string VehicleNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Make { get; set; }

        [MaxLength(50)]
        public string? Model { get; set; }

        [MaxLength(30)]
        public string? Color { get; set; }
    }
}