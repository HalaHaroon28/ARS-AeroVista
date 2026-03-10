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
    }
}