using System;
using System.ComponentModel.DataAnnotations;
using AeroVista.Models;

namespace AeroVista.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public Flights Flight { get; set; } = default!;

        public int Adults { get; set; }
        public int Children { get; set; }

        public string TravelClass { get; set; } = "Economy";

        public decimal TotalPrice { get; set; }

        public DateTime BookingDate { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public string PaymentMethod { get; set; } = "Pending";
        public string TransactionId { get; set; } = "Pending";

        public string Status { get; set; } = "Pending";
    }
}