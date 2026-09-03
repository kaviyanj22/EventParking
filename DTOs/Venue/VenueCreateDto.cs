using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Venue
{
    public class VenueCreateDto
    {
        [Required(ErrorMessage = "Venue name is required.")]
        [MaxLength(150, ErrorMessage = "Venue name cannot exceed 150 characters.")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }
    }
}