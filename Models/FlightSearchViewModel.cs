using System.ComponentModel.DataAnnotations;

namespace AeroVista.Models
{
    public class FlightSearchViewModel
    {
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        public string TripType { get; set; } = "RoundTrip";

        // This is the new property for the +5 days logic
        public bool IsFlexible { get; set; }

        public int Adults { get; set; } = 1;
        public int Children { get; set; } = 0;

        public string TravelClass { get; set; } = "Economy";

        public List<City>? Cities { get; set; }
    }
}