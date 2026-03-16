using System.ComponentModel.DataAnnotations;

namespace AeroVista.Models
{
    public class ProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int SkyMiles { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [CreditCard]
        public string PreferredCardNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}