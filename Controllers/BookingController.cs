using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AeroVista.Controllers
{

    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Book(int flightId, int adults, int children, string travelClass)
        {
            return RedirectToAction("Confirmation", new { id = flightId, adults, children, travelClass });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id, int adults, int children, string travelClass)
        {
            var flight = await _context.Flights
                .Include(f => f.FromCity)
                .Include(f => f.ToCity)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            decimal basePrice = (flight.Price * adults) + (flight.Price * 0.7m * children);

            if (travelClass == "First")
            {
                basePrice *= 1.50m;
            }
            else if (travelClass == "Business")
            {
                basePrice *= 1.30m;
            }

            var booking = new Booking
            {
                FlightId = id,
                Flight = flight,
                Adults = adults,
                Children = children,
                TravelClass = travelClass,
                TotalPrice = basePrice
            };

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPayment(int flightId, int adults, int children, string travelClass, string paymentMethod, string transactionId, decimal totalPrice)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var booking = new Booking
            {
                FlightId = flightId,
                UserId = user.Id,
                Adults = adults,
                Children = children,
                TravelClass = travelClass,
                BookingDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = totalPrice,
                TransactionId = transactionId,
                PaymentMethod = paymentMethod

            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
           
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var bookings = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return RedirectToAction(nameof(MyBookings));

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancellation(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return NotFound();

            booking.Status = "Cancelled";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> Status(int? id)
        {
            if (id == null) return RedirectToAction(nameof(MyBookings));

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        public async Task<IActionResult> Reschedule(int? id)
        {
            if (id == null) return RedirectToAction(nameof(MyBookings));

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessReschedule(int bookingId, DateTime newDate)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return NotFound();

            booking.Status = "Rescheduled";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Status", new { id = bookingId });

        }
    }
}
