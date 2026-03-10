using System.ComponentModel.DataAnnotations;
namespace AeroVista.Models


{
    public class City
    {
        [Required]
        public int CityId { get; set; }
        [Required]
        public string CityName  { get; set; } = default!;
        [Required]
        public string AirportCode { get; set; } = default!;
        [Required]
        public string Country { get; set; } = default!;
    }
}
