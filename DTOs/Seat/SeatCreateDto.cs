using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Seat
{
    public class SeatCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? RowName { get; set; }

        public int? ColumnNumber { get; set; }

        [MaxLength(50)]
        public string? SeatType { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
    }
}