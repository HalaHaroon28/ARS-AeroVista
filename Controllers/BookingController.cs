using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroVista.Data;
using AeroVista.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AeroVista.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GenerateRef(string prefix) =>
            $"{prefix}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        [HttpGet]
        public async Task<IActionResult> Status(int? id)
        {
            if (id == null) return View();

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Reschedule(int? id)
        {
            if (id == null) return View();

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return View();

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> ViewByReference(string referenceNumber)
        {
            if (string.IsNullOrEmpty(referenceNumber))
            {
                TempData["Error"] = "Please enter a valid reference sequence.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var booking = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .FirstOrDefaultAsync(b => b.ConfirmationNumber == referenceNumber ||
                                          b.BlockingNumber == referenceNumber ||
                                          b.CancellationNumber == referenceNumber);

            if (booking == null)
            {
                TempData["Error"] = "Reference not found in AeroVista archives.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            string referer = Request.Headers["Referer"].ToString().ToLower();
            if (referer.Contains("reschedule")) return View("Reschedule", booking);
            if (referer.Contains("cancel")) return View("Cancel", booking);

            return View("Status", booking);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Book(int flightId, int adults, int children, string travelClass, string actionType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (actionType == "Block")
            {
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null) return NotFound();

                if ((flight.DepartureTime - DateTime.Now).TotalDays <= 14)
                {
                    TempData["Error"] = "Blocking is only available for flights departing in more than 14 days.";
                    return RedirectToAction("Results", "Flights");
                }

                var booking = new Booking
                {
                    FlightId = flightId,
                    UserId = user.Id,
                    Adults = adults,
                    Children = children,
                    TravelClass = travelClass,
                    BookingDate = DateTime.Now,
                    Status = "Blocked",
                    BlockingNumber = GenerateRef("BLK")
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                TempData["BookingMessage"] = "Ticket successfully blocked.";
                return RedirectToAction(nameof(MyBookings));
            }

            return RedirectToAction("Confirmation", new { id = flightId, adults, children, travelClass });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id, int adults, int children, string travelClass, int? bookingId = null)
        {
            var flight = await _context.Flights
                .Include(f => f.FromCity)
                .Include(f => f.ToCity)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            decimal basePrice = (flight.Price * adults) + (flight.Price * 0.7m * children);
            if (travelClass == "First") basePrice *= 1.50m;
            else if (travelClass == "Business") basePrice *= 1.30m;

            var booking = new Booking
            {
                Id = bookingId ?? 0,
                FlightId = id,
                Flight = flight,
                Adults = adults,
                Children = children,
                TravelClass = travelClass,
                TotalPrice = basePrice
            };

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPayment(int flightId, int adults, int children, string travelClass, string paymentMethod, string transactionId, decimal totalPrice, int? existingBookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            Booking booking;

            if (existingBookingId.HasValue && existingBookingId.Value > 0)
            {
                booking = await _context.Bookings.FindAsync(existingBookingId.Value);
                if (booking == null) return NotFound();
            }
            else
            {
                booking = new Booking { UserId = user.Id, FlightId = flightId };
                _context.Bookings.Add(booking);
            }

            booking.Adults = adults;
            booking.Children = children;
            booking.TravelClass = travelClass;
            booking.BookingDate = DateTime.Now;
            booking.Status = "Pending";
            booking.TotalPrice = totalPrice;
            booking.TransactionId = transactionId;
            booking.PaymentMethod = paymentMethod;
            booking.ConfirmationNumber = GenerateRef("AV");

            user.SkyMiles += ((adults + children) * 50);
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.FromCity)
                .Include(b => b.Flight).ThenInclude(f => f.ToCity)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancellation(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return NotFound();

            booking.Status = "Cancelled";
            booking.CancellationNumber = GenerateRef("CNL");

            _context.Update(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyBookings));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessReschedule(int bookingId, DateTime newDate)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return NotFound();

            booking.Status = "Rescheduled";
            booking.IsRescheduled = true;

            _context.Update(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Status", new { id = bookingId });
        }
    }
}