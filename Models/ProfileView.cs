using System.ComponentModel.DataAnnotations;

namespace AeroVista.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Sex { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password to Save Changes")]
        public string ConfirmPassword { get; set; } = default!;
    }
}