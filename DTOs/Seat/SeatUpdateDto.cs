using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Seat
{
    public class SeatUpdateDto
    {
        [Range(1, int.MaxValue)]
        public int? SeatSectionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PositionX { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PositionY { get; set; }

        [MaxLength(50)]
        public string? SeatType { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";
    }
}