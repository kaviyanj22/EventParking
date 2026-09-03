using System.ComponentModel.DataAnnotations;

namespace Event_parking.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public int CustomerId { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; } = null!;
    }
}