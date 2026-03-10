using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AeroVista.Controllers
{
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlightsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Results(FlightSearchViewModel model, string sortOrder, decimal? maxPrice, int page = 1)
        {
            int pageSize = 5;
            var query = _context.Flights
                .Include(f => f.FromCity).Include(f => f.ToCity)
                .Where(f => f.FromCityId == model.FromCityId &&
                            f.ToCityId == model.ToCityId &&
                            f.DepartureTime.Date == model.DepartureDate.Date &&
                            f.SeatsAvailable >= (model.Adults + model.Children));

            if (maxPrice.HasValue) query = query.Where(f => f.Price <= maxPrice.Value);

            int totalFlights = await query.CountAsync();
            var flights = await query.OrderBy(f => f.Price).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            List<Flights> returnFlights = new();
            if (model.TripType == "RoundTrip" && model.ReturnDate.HasValue)
            {
                returnFlights = await _context.Flights
                    .Include(f => f.FromCity).Include(f => f.ToCity)
                    .Where(f => f.FromCityId == model.ToCityId &&
                                f.ToCityId == model.FromCityId &&
                                f.DepartureTime.Date == model.ReturnDate.Value.Date &&
                                f.SeatsAvailable >= (model.Adults + model.Children))
                    .ToListAsync();
            }

            ViewBag.ReturnFlights = returnFlights;
            ViewBag.SearchModel = model;
            ViewBag.TotalPages = (int)Math.Ceiling(totalFlights / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(flights);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Book(int flightId, int adults, int children, string travelClass)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            int totalPax = adults + children;

            if (flight.SeatsAvailable < totalPax) return BadRequest("Seats no longer available.");

            decimal basePrice = flight.Price;
            decimal classSurcharge = travelClass switch
            {
                "Business" => 50000,
                "First" => 80000,
                _ => 0
            };

            decimal adultPriceTotal = basePrice + classSurcharge;
            decimal childPriceTotal = (basePrice * 0.5m) + classSurcharge;
            decimal finalTotal = (adults * adultPriceTotal) + (children * childPriceTotal);

            var booking = new Booking
            {
                FlightId = flightId,
                UserId = user.Id,
                Adults = adults,
                Children = children,
                TravelClass = travelClass,
                TotalPrice = finalTotal,
                BookingDate = DateTime.Now,
                Status = "Pending"
            };

            flight.SeatsAvailable -= totalPax;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", "Booking", new { id = booking.Id });
        }
    }
}