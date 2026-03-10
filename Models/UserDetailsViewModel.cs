using Microsoft.AspNetCore.Identity;

namespace AeroVista.Models
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Claims { get; set; } = new List<string>();

        // Add custom properties from your ApplicationUser
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
    }
}