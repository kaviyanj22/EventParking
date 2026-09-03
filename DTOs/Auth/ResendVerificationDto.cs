using System.ComponentModel.DataAnnotations;

namespace Event_parking.DTOs.Auth
{
    public class ResendVerificationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}