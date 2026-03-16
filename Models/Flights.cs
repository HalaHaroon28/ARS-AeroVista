using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroVista.Models
{
    public class Flights
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = default!;

        [Required]
        public int FromCityId { get; set; }

        [ForeignKey("FromCityId")]
        public City FromCity { get; set; } = default!;

        [Required]
        public int ToCityId { get; set; }

        [ForeignKey("ToCityId")]
        public City ToCity { get; set; } = default!;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }


        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
    }
}