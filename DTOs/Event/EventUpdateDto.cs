using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Event
{
    public class EventUpdateDto
    {
        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(150, ErrorMessage = "Event name cannot exceed 150 characters.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Venue is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid venue.")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan EndTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ticket price cannot be negative.")]
        public decimal TicketPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Parking fee cannot be negative.")]
        public decimal ParkingFee { get; set; }
    }
}