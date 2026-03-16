using System.ComponentModel.DataAnnotations;

namespace AeroVista.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string ContactNo { get; set; }
        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
