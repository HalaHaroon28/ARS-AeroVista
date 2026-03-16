using System;
using System.ComponentModel.DataAnnotations;

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

        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? CardLastFour { get; set; } 

        
        public string Status { get; set; } = "Pending"; 
        public string? ConfirmationNumber { get; set; }
        public string? BlockingNumber { get; set; }
        public string? CancellationNumber { get; set; }

      
        public bool IsRescheduled { get; set; } = false;
        public int? PreviousBookingId { get; set; }
    }
}