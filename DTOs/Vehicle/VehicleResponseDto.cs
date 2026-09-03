namespace Event_parking.DTOs.Vehicle
{
    public class VehicleResponseDto
    {
        public int VehicleId { get; set; }

        public int CustomerId { get; set; }

        public string VehicleType { get; set; } = string.Empty;

        public string VehicleNumber { get; set; } = string.Empty;

        public string? Make { get; set; }

        public string? Model { get; set; }

        public string? Color { get; set; }
    }
}