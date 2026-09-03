using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Customer
{
    public class CustomerUpdateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}