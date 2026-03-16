using Microsoft.AspNetCore.Identity;
using System;

namespace AeroVista.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Sex { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }
        public string? PreferredCardNumber { get; set; }
        public int SkyMiles { get; set; } = 0;
    }
}