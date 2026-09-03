using System.ComponentModel.DataAnnotations;

namespace Event_parking.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Customer";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public bool EmailVerified { get; set; } = false;

        public string? EmailVerificationTokenHash { get; set; }

        public DateTime? EmailVerificationTokenExpiresAt { get; set; }

        public string? PasswordResetTokenHash { get; set; }

        public DateTime? PasswordResetTokenExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
            = new List<Vehicle>();
    }
}